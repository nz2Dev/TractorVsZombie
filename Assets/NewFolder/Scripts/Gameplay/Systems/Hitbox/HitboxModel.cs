using UnityEngine;

internal sealed class HitboxModel {
    public HitboxModel(HitboxId id, FactionState factionState, RaycastId raycastId, ProximityId proximityId, PoseId? poseId, Vector3 position) {
        Id = id;
        FactionState = factionState;
        RaycastId = raycastId;
        ProximityId = proximityId;
        PoseId = poseId;
        Position = position;
    }

    public HitboxId Id { get; }
    public FactionState FactionState { get; }
    public RaycastId RaycastId { get; }
    public ProximityId ProximityId { get; }
    public PoseId? PoseId { get; }
    public Vector3 Position { get; set; }
}
