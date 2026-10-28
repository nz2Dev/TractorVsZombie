using UnityEngine;

public sealed class CarryingModel {
    public CarryingId Id { get; }
    public PoseId HostPoseId { get; }
    public Vector3 LoadoutOffset { get; }
    public int LoadoutId { get; set; }

    public CarryingModel(CarryingId id, PoseId hostPoseId, Vector3 loadoutOffset) {
        Id = id;
        HostPoseId = hostPoseId;
        LoadoutOffset = loadoutOffset;
    }
}
