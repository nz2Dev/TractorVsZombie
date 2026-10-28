using System.Collections.Generic;
using UnityEngine;

public sealed class AttachmentSystem {
    private readonly PoseRegistry poseRegistry;
    private readonly WeaponController weaponController;
    private readonly Dictionary<AttachmentId, AttachmentModel> models = new();
    private int idCounter;

    public AttachmentSystem(PoseRegistry poseRegistry, WeaponController weaponController) {
        this.poseRegistry = poseRegistry;
        this.weaponController = weaponController;
    }

    public AttachmentId Add(AttachmentPrototype prototype, FactionId ownerFactionId, PoseId hostPoseId) {
        var id = new AttachmentId(++idCounter);
        var weaponId = weaponController.SpawnWeapon(ownerFactionId, prototype.weaponPrototype);
        models.Add(id, new AttachmentModel(id, weaponId, hostPoseId, prototype.weaponPlacementOffset));
        return id;
    }

    public void Remove(AttachmentId id) {
        if (!models.Remove(id, out var model))
            return;
        weaponController.DeleteWeapon(model.WeaponId);
    }

    public AttachmentState ReadState(AttachmentId id) {
        var model = models[id];
        return new AttachmentState {
            weaponId = model.WeaponId,
            weaponState = weaponController.ReadWeaponState(model.WeaponId),
        };
    }

    public void Update() {
        foreach (var model in models.Values) {
            var pose = poseRegistry.Read(model.HostPoseId);
            weaponController.MoveWeapon(model.WeaponId, pose.position + pose.rotation * model.WeaponPlacementOffset);
        }
    }
}
