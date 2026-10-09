using System.Collections.Generic;

using Combat;

using Interactions;

using UnityEngine;

public class MotionSystem {

    private readonly CombatSystem combatSystem;
    private readonly RagdollService ragdollService;
    private readonly CollisionService collisionService;
    private readonly PoseRegistry poseRegistry;

    private readonly Dictionary<MotionId, MotionModel> registry = new();
    private int idCounter;

    public MotionSystem(RagdollService ragdollService, CollisionService collisionService, CombatSystem combatSystem, PoseRegistry poseRegistry) {
        this.ragdollService = ragdollService;
        this.collisionService = collisionService;
        this.combatSystem = combatSystem;
        this.poseRegistry = poseRegistry;
    }

    public MotionId Add(PoseId poseId, Vector3 position, InfantryConfig config, RagdollBody bodyPrefab, CombatId hostCombatId) {
        var id = new MotionId(++idCounter);
        var bodyId = ragdollService.RegisterPhysicsEntity(position, bodyPrefab);
        registry[id] = new MotionModel(id, poseId, bodyId, config, hostCombatId);
        return id;
    }

    public void Remove(MotionId id) {
        if (registry.Remove(id, out var model)) {
            ragdollService.UnregisterPhysicsEntity(model.BodyId);
        }
    }

    public void AddExplosionEffect(MotionId id, Explosion explosion) {
        var model = registry[id];
        model.OccurredEffectType = EffectType.Explosion;
        model.ExplosionData = explosion;
    }

    public MotionState ReadState(MotionId id) {
        var model = registry[id];
        return new MotionState {
            isGrounded = model.Grounded,
        };
    }

    public void Update() {
        foreach (var model in registry.Values) {
            var physicsPose = ragdollService.GetEntityPose(model.BodyId);
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
                poseRegistry.Write(model.HostPoseId, 
                    physicsPose.Position, physicsPose.Rotation);
            } else if (becomeGrounded) {
                model.Grounded = true;
                
                var combatState = combatSystem.ReadState(model.HostCombatId);
                if (!combatState.isDead) {
                    ragdollService.SetPhysicsActive(model.BodyId, false);
                    var groundPosition = collisionService.GetClosestVerticalGroundPoint(physicsPose.Position);
                    var defaulRotation = Quaternion.identity;
                    poseRegistry.Write(model.HostPoseId, 
                        groundPosition, defaulRotation);
                }
            }

            model.ActiveEffectType = model.OccurredEffectType;
            model.OccurredEffectType = EffectType.None;

            if (model.ActiveEffectType == EffectType.Explosion) {
                model.Grounded = false;
                model.UnsettleStartTime = Time.time;

                var pose = poseRegistry.Read(model.HostPoseId);
                ragdollService.SetPhysicsActive(model.BodyId, true);
                // setting rotation is missing, TODO: check that
                ragdollService.UpdatePhysicsEntityPosition(model.BodyId, pose.position);
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
