using System;
using System.Collections.Generic;


using UnityEngine;

public class PlatformController {

    private readonly FactionRegistry factionRegistry;
    private readonly CarryingSystem carryingSystem;
    private readonly VehicleSystem vehicleSystem;
    private readonly PoseRegistry poseRegistry;
    private readonly PlatformPresenter presenter;

    private int idCounter;
    private readonly Dictionary<int, PlatformModel> registry = new();

    public PlatformController(FactionRegistry factionRegistry, CarryingSystem carryingSystem, VehicleSystem vehicleSystem, PlatformPresenter presenter, PoseRegistry poseRegistry) {
        this.factionRegistry = factionRegistry;
        this.carryingSystem = carryingSystem;
        this.vehicleSystem = vehicleSystem;
        this.presenter = presenter;
        this.poseRegistry = poseRegistry;
    }

    public void Update() {
        UpdatePresentation();
        RemoveDestroyed();
    }

    public virtual int Create(PlatformPrototype prototype, Vector3 position = default) {
        var nextId = ++idCounter;
        var initPosition = position == default ? prototype.position : position;
        var model = new PlatformModel(nextId);
        registry[model.Id] = model;

        model.PoseId = poseRegistry.Add(initPosition, default);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        model.CarryingId = carryingSystem.Add(prototype.carryingPrototype, model.PoseId);
        model.VehicleId = vehicleSystem.Create(initPosition, prototype.vehiclePrototype, poseId: model.PoseId);
        
        presenter.Add(model.Id, initPosition, prototype.presentationPrototype);
        return model.Id;
    }

    internal bool Exist(int platformId) {
        return registry.ContainsKey(platformId);
    }

    public virtual void SetLoadout(int platformId, LoadoutPrototype loadoutPrototype) {
        var platform = registry[platformId];
        carryingSystem.SetLoadout(platform.CarryingId, platform.FactionId, loadoutPrototype);
    }

    public virtual VehicleId ReadVehicleId(int platformId) {
        return registry[platformId].VehicleId;
    }

    public void ReadAllPlatforms(IList<PlatformState> statesBuffer) {
        statesBuffer.Clear();
        foreach (var platform in registry.Values) {
            statesBuffer.Add(ReadPlatformState(platform));
        }
    }

    public virtual PlatformState ReadPlatformState(int platformId) {
        return ReadPlatformState(registry[platformId]);
    }

    public virtual PlatformState ReadPlatformState(PlatformModel platform) {
        var carryingState = carryingSystem.ReadState(platform.CarryingId);
        var pose = poseRegistry.Read(platform.PoseId);
        return new PlatformState (
            position: pose.position,
            loadoutState: carryingState.loadoutState,
            hasLoadout: carryingState.hasLoadout,
            vehicleId: platform.VehicleId,
            platformId: platform.Id
        );
    }

    private List<int> removalBuffer = new();

    private void RemoveDestroyed() {
        removalBuffer.Clear();
        foreach (var platform in registry.Values) {
            var carryingState = carryingSystem.ReadState(platform.CarryingId);
            if (carryingState.hasLoadout && carryingState.loadoutState.healthState.isDead)
                removalBuffer.Add(platform.Id);
        }

        foreach (var destroyed in removalBuffer) {
            Remove(destroyed);
        }
    }

    private void Remove(int platformId) {
        registry.Remove(platformId, out var model);
        carryingSystem.Remove(model.CarryingId);
        factionRegistry.Remove(model.FactionId);
        vehicleSystem.Remove(model.VehicleId);
        poseRegistry.Remove(model.PoseId);
        
        presenter.Remove(platformId);
    }

    private void UpdatePresentation() {
        foreach (var host in registry.Values) {
            var vehicleState = vehicleSystem.ReadState(host.VehicleId);
            var carryingState = carryingSystem.ReadState(host.CarryingId);
            presenter.Update(host.Id, vehicleState, carryingState);
        }
    }

}
