using UnityEngine;

internal sealed class HitboxModel {
    public HitboxModel(HitboxId id, FactionState factionState, RaycastId raycastId, RaycastState raycastState, ProximityId proximityId, PoseId? poseId, Vector3 position, ContactSurface surface) {
        Id = id;
        FactionState = factionState;
        RaycastId = raycastId;
        RaycastState = raycastState;
        ProximityId = proximityId;
        PoseId = poseId;
        Position = position;
        Surface = surface;
    }

    public HitboxId Id { get; }
    public FactionState FactionState { get; }
    public RaycastId RaycastId { get; }
    public RaycastState RaycastState { get; }
    public ProximityId ProximityId { get; }
    public PoseId? PoseId { get; }
    public Vector3 Position { get; set; }
    public ContactSurface Surface { get; }
}
