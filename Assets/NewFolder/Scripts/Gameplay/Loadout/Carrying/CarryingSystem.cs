using System.Collections.Generic;
using UnityEngine;

public sealed class CarryingSystem {
    private readonly PoseRegistry poseRegistry;
    private readonly LoadoutController loadoutController;
    private readonly Dictionary<CarryingId, CarryingModel> models = new();
    private int idCounter;

    public CarryingSystem(PoseRegistry poseRegistry, LoadoutController loadoutController) {
        this.poseRegistry = poseRegistry;
        this.loadoutController = loadoutController;
    }

    public CarryingId Add(CarryingPrototype prototype, PoseId hostPoseId) {
        var id = new CarryingId(++idCounter);
        models.Add(id, new CarryingModel(id, hostPoseId, prototype.loadoutOffset));
        return id;
    }

    public void SetLoadout(CarryingId id, FactionId ownerFactionId, LoadoutPrototype prototype) {
        var model = models[id];
        if (model.LoadoutId != 0)
            loadoutController.DeleteLoadout(model.LoadoutId);

        var pose = poseRegistry.Read(model.HostPoseId);
        prototype.position = pose.position + model.LoadoutOffset;
        model.LoadoutId = loadoutController.SpawnLoadout(ownerFactionId, prototype);
    }

    public void Remove(CarryingId id) {
        models.Remove(id, out var model);
        if (model.LoadoutId != 0)
            loadoutController.DeleteLoadout(model.LoadoutId);
    }

    public CarryingState ReadState(CarryingId id) {
        var model = models[id];
        var state = new CarryingState { loadoutId = model.LoadoutId };
        if (model.LoadoutId == 0)
            return state;

        var loadoutState = loadoutController.ReadLoadoutState(model.LoadoutId);
        state.weaponId = loadoutState.weaponId;
        state.weaponState = loadoutState.weaponState;
        return state;
    }

    public void Update() {
        foreach (var model in models.Values) {
            if (model.LoadoutId == 0)
                continue;
                
            var pose = poseRegistry.Read(model.HostPoseId);
            loadoutController.MoveLoadout(model.LoadoutId, pose.position + model.LoadoutOffset, pose.rotation);
        }
    }
}
