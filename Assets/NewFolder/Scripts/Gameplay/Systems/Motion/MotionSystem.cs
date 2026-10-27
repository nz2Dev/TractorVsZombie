using System.Collections.Generic;


using Interactions;

using UnityEngine;

public class MotionSystem {

    private readonly HealthSystem healthSystem;
    private readonly RagdollService ragdollService;
    private readonly CollisionService collisionService;
    private readonly PoseRegistry poseRegistry;

    private readonly Dictionary<MotionId, MotionModel> registry = new();
    private int idCounter;

    public MotionSystem(RagdollService ragdollService, CollisionService collisionService, HealthSystem healthSystem, PoseRegistry poseRegistry) {
        this.ragdollService = ragdollService;
        this.collisionService = collisionService;
        this.healthSystem = healthSystem;
        this.poseRegistry = poseRegistry;
    }

    public MotionId Add(PoseId poseId, Vector3 position, MotionPrototype prototype, HealthId hostHealthId) {
        var id = new MotionId(++idCounter);
        var bodyId = ragdollService.RegisterPhysicsEntity(position, prototype.physicsBodyPrefab);
        registry[id] = new MotionModel(id, poseId, bodyId, prototype.config, hostHealthId);
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
                
                var healthState = healthSystem.ReadState(model.HostHealthId);
                if (!healthState.isDead) {
                    ragdollService.SetPhysicsActive(model.BodyId, false);
                    var groundPosition = collisionService.GetClosestVerticalGroundPoint(physicsPose.Position);
                    var defaulRotation = Quaternion.identity;
                    poseRegistry.Write(model.HostPoseId, 
                        groundPosition, defaulRotation);
                    ragdollService.UpdateEntityPose(model.BodyId, groundPosition, defaulRotation);
                }
            } else if (keepsGrounded) {
                var pose = poseRegistry.Read(model.HostPoseId);
                var depenetrationVector = ragdollService.GetDepenetrationVector(model.BodyId);
                // project on plane is costly, consider simply flatenning Y
                depenetrationVector = Vector3.ProjectOnPlane(depenetrationVector, Vector3.up);
                var correctedPosition = pose.position + depenetrationVector;
                poseRegistry.Write(model.HostPoseId, correctedPosition, pose.rotation);
                ragdollService.UpdatePhysicsEntityPosition(model.BodyId, correctedPosition);
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
