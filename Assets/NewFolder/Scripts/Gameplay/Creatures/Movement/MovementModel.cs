using UnityEngine;

internal sealed class MovementModel {
    public MovementId Id { get; }
    public MotionId HolderMotionId { get; }
    public PoseId HolderPoseId { get; }
    public int AvoidanceId { get; }
    public float MaxSpeed { get; }
    public MovementConfig Config { get; }

    public Vector3 Velocity { get; set; }
    public Vector3? MoveDestination { get; set; }
    public bool HoldMovement { get; set; }

    public MovementModel(MovementId id, MotionId holderMotionId, PoseId holderPoseId, int avoidanceId, float maxSpeed, MovementConfig config) {
        Id = id;
        HolderMotionId = holderMotionId;
        HolderPoseId = holderPoseId;
        AvoidanceId = avoidanceId;
        MaxSpeed = maxSpeed;
        Config = config;
    }
}
