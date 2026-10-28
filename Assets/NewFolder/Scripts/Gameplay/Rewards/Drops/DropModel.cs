using UnityEngine;

public sealed class DropModel {

    public DropId Id { get; }
    public HealthId HostHealthId { get; }
    public PoseId HostPoseId { get; }
    
    public RewardPrototype RewardPrototype { get; }
    public bool InheritHostRotation { get; }
    public bool HasDropped { get; set; }

    public DropModel(DropId id, RewardPrototype rewardPrototype, bool inheritHostRotation, HealthId hostHealthId, PoseId hostPoseId) {
        Id = id;
        RewardPrototype = rewardPrototype;
        InheritHostRotation = inheritHostRotation;
        HostHealthId = hostHealthId;
        HostPoseId = hostPoseId;
    }
    
}
