using System.Collections.Generic;

using UnityEngine;

public sealed class MovementSystem {
    private readonly MotionSystem motionSystem;
    private readonly PoseRegistry poseRegistry;
    private readonly AvoidanceService avoidanceService;
    private readonly Dictionary<MovementId, MovementModel> registry = new();
    private int idCounter;

    public MovementSystem(MotionSystem motionSystem, PoseRegistry poseRegistry, AvoidanceService avoidanceService) {
        this.motionSystem = motionSystem;
        this.poseRegistry = poseRegistry;
        this.avoidanceService = avoidanceService;
    }

    public MovementId Add(Vector3 position, AgentAvoidanceConfig avoidanceConfig, MotionId holderMotionId, PoseId holderPoseId, InfantryConfig config) {
        var id = new MovementId(++idCounter);
        var avoidanceId = avoidanceService.AddAgent(position, avoidanceConfig);
        registry[id] = new MovementModel(id, holderMotionId, holderPoseId, avoidanceId, avoidanceConfig.maxSpeed, config);
        return id;
    }

    public void Remove(MovementId id) {
        if (registry.Remove(id, out var model)) {
            avoidanceService.RemoveAgent(model.AvoidanceId);
        }
    }

    public void MoveTo(MovementId id, Vector3 destination) {
        registry[id].MoveDestination = destination;
    }

    public void StopMovement(MovementId id) {
        registry[id].MoveDestination = null;
    }

    public void HoldMovement(MovementId id) {
        registry[id].HoldMovement = true;
    }

    public MovementState ReadState(MovementId id) {
        var model = registry[id];
        return new MovementState {
            velocity = model.Velocity,
            maxSpeed = model.MaxSpeed
        };
    }

    public void Update() {
        foreach (var model in registry.Values) {
            var pose = poseRegistry.Read(model.HolderPoseId);
            var motionState = motionSystem.ReadState(model.HolderMotionId);
            var rvoVelocity = avoidanceService.GetVelocity(model.AvoidanceId);

            if (motionState.isGrounded) {
                model.Velocity = rvoVelocity;
                pose.position += rvoVelocity * Time.deltaTime;
                avoidanceService.SetAgentPosition(model.AvoidanceId, pose.position);

                if (rvoVelocity.sqrMagnitude > float.Epsilon) {
                    pose.rotation = Quaternion.LookRotation(rvoVelocity.normalized, Vector3.up);
                }

                poseRegistry.Write(model.HolderPoseId, pose.position, pose.rotation);
            }

            var canMove = motionState.isGrounded && model.MoveDestination.HasValue && !model.HoldMovement;
            model.HoldMovement = false;

            var preferedVelocity = Vector3.zero;
            if (canMove) {
                var moveVelocity = model.MoveDestination.Value - pose.position;
                var distance = moveVelocity.magnitude;
                var speedFactor = Mathf.Clamp01(distance / model.Config.stoppingDistance);
                preferedVelocity = distance > float.Epsilon ? moveVelocity / distance * model.MaxSpeed * speedFactor : Vector3.zero;
            }

            avoidanceService.SetPreferedVelocity(model.AvoidanceId, preferedVelocity);
        }
    }
}
