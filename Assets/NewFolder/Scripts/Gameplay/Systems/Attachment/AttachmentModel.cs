using UnityEngine;

public sealed class AttachmentModel {
    public AttachmentId Id { get; }
    public int WeaponId { get; }
    public PoseId HostPoseId { get; }
    public Vector3 WeaponPlacementOffset { get; }

    public AttachmentModel(AttachmentId id, int weaponId, PoseId hostPoseId, Vector3 weaponPlacementOffset) {
        Id = id;
        WeaponId = weaponId;
        HostPoseId = hostPoseId;
        WeaponPlacementOffset = weaponPlacementOffset;
    }
}
