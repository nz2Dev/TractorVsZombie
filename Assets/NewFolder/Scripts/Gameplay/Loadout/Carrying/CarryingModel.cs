using UnityEngine;

public sealed class CarryingModel {
    
    public CarryingId Id { get; }
    public PoseId HostPoseId { get; }
    public Vector3 LoadoutOffset { get; }
    public int LoadoutId { get; set; }
    public CarryingConfig Config { get; }
    public int Durability { get; set; }

    public CarryingModel(CarryingId id, PoseId hostPoseId, Vector3 loadoutOffset, CarryingConfig config) {
        Id = id;
        HostPoseId = hostPoseId;
        LoadoutOffset = loadoutOffset;
        Config = config;
    }

}
