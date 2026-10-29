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
        var model = new CarryingModel(id, hostPoseId, prototype.loadoutOffset, prototype.config);
        model.Durability = model.Config.maxDurabilty;
        models[id] = model;
        return id;
    }

    public void SetLoadout(CarryingId id, FactionId factionId, LoadoutPrototype prototype) {
        var model = models[id];
        if (model.LoadoutId != 0 && loadoutController.Exists(model.LoadoutId))
            loadoutController.RemoveLoadout(model.LoadoutId);

        var pose = poseRegistry.Read(model.HostPoseId);
        prototype.position = pose.position + model.LoadoutOffset;
        model.LoadoutId = loadoutController.SpawnLoadout(factionId, prototype);
    }

    public void Remove(CarryingId id) {
        models.Remove(id, out var model);
        if (model.LoadoutId != 0 && loadoutController.Exists(model.LoadoutId))
            loadoutController.RemoveLoadout(model.LoadoutId);
    }

    public CarryingState ReadState(CarryingId id) {
        var model = models[id];
        var state = new CarryingState {
            durability = model.Durability,
            maxDurability = model.Config.maxDurabilty,
            destroyed = model.Durability == 0,
        };

        if (model.LoadoutId == 0 || !loadoutController.Exists(model.LoadoutId))
            return state;

        state.hasLoadout = true;
        state.loadoutId = model.LoadoutId;
        state.loadoutState = loadoutController.ReadLoadoutState(model.LoadoutId);
        return state;
    }

    public void Update() {
        foreach (var model in models.Values) {
            if (model.LoadoutId == 0)
                continue;

            if (!loadoutController.Exists(model.LoadoutId)) {
                if (model.Durability > 0)
                    model.Durability--;
                model.LoadoutId = 0;
                continue;
            }
                
            var pose = poseRegistry.Read(model.HostPoseId);
            loadoutController.MoveLoadout(model.LoadoutId, pose.position + model.LoadoutOffset, pose.rotation);
        }
    }
}
