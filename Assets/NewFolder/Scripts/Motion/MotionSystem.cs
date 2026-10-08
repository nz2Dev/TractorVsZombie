using System.Collections.Generic;

using Combat;

using Interactions;

using UnityEngine;

public class MotionSystem {

    private readonly CombatSystem combatSystem;
    private readonly RagdollService ragdollService;
    private readonly CollisionService collisionService;
    private readonly Dictionary<MotionId, MotionModel> registry = new();
    private int idCounter;

    public MotionSystem(RagdollService ragdollService, CollisionService collisionService, CombatSystem combatSystem) {
        this.ragdollService = ragdollService;
        this.collisionService = collisionService;
        this.combatSystem = combatSystem;
    }

    public MotionId Add(Vector3 position, InfantryConfig config, RagdollBody bodyPrefab, CombatId hostCombatId) {
        var id = new MotionId(++idCounter);
        var bodyId = ragdollService.RegisterPhysicsEntity(position, bodyPrefab);
        registry[id] = new MotionModel(id, bodyId, config, position, hostCombatId);
        return id;
    }

    public void Remove(MotionId id) {
        if (registry.Remove(id, out var model)) {
            ragdollService.UnregisterPhysicsEntity(model.BodyId);
        }
    }

    public void SetPose(MotionId id, Vector3 position, Quaternion rotation) {
        var model = registry[id];
        model.Position = position;
        model.Rotation = rotation;
    }

    public void AddExplosionEffect(MotionId id, Explosion explosion) {
        var model = registry[id];
        model.OccurredEffectType = EffectType.Explosion;
        model.ExplosionData = explosion;
    }

    public MotionState ReadState(MotionId id) {
        var model = registry[id];
        return new MotionState {
            position = model.Position,
            rotation = model.Rotation,
            isGrounded = model.Grounded,
            becameGrounded = model.BecameGrounded
        };
    }

    public void Update() {
        foreach (var model in registry.Values) {
            var combatState = combatSystem.ReadState(model.HostCombatId);
            var physicsPose = ragdollService.GetEntityPose(model.BodyId);
            model.BecameGrounded = false;

            if (model.OnTheFloor && !physicsPose.ContactWithGround) {
                model.OnTheFloor = false;
            } else if (!model.OnTheFloor && physicsPose.ContactWithGround) {
                model.OnTheFloor = true;
                model.ContactWithGroundStartTime = Time.time;
            }

            var inMotion = physicsPose.Velocity.sqrMagnitude > model.Config.settleSpeedSquaredThreashold;
            var minUnsettleTimeReached = Time.time - model.UnsettleStartTime > model.Config.minUnsettleTimeSec;
            var maxTimeOnTheFloorReached = Time.time - model.ContactWithGroundStartTime > model.Config.maxTimeOnTheFloor;
            // FIXME: !inMotion can oocure mid-fly, so should be combined with onTheFloor part.
            var settled = !inMotion && minUnsettleTimeReached || model.OnTheFloor && maxTimeOnTheFloorReached;

            var keepFlying = !model.Grounded && !settled;
            var becomeGrounded = !model.Grounded && settled;
            var keepsGrounded = model.Grounded && settled;

            if (keepFlying) {
                model.Position = physicsPose.Position;
                model.Rotation = physicsPose.Rotation;
            } else if (becomeGrounded) {
                model.Grounded = true;
                model.BecameGrounded = true;
                
                if (!combatState.isDead) {
                    ragdollService.SetPhysicsActive(model.BodyId, false);
                    model.Position = collisionService.GetClosestVerticalGroundPoint(physicsPose.Position);
                    model.Rotation = Quaternion.identity;
                }
            }

            model.ActiveEffectType = model.OccurredEffectType;
            model.OccurredEffectType = EffectType.None;

            if (model.ActiveEffectType == EffectType.Explosion) {
                model.Grounded = false;
                model.BecameGrounded = false;
                model.UnsettleStartTime = Time.time;
                ragdollService.SetPhysicsActive(model.BodyId, true);
                ragdollService.UpdatePhysicsEntityPosition(model.BodyId, model.Position);
                ragdollService.AddExplosionForce(
                    model.BodyId,
                    model.ExplosionData.config.force,
                    model.ExplosionData.epicentr,
                    model.ExplosionData.config.radius,
                    model.ExplosionData.config.upwardModifier,
                    model.ExplosionData.config.forceMode
                );
            }
        }
    }
}
