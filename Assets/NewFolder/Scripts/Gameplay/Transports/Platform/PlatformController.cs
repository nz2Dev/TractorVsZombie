using System;
using System.Collections.Generic;


using UnityEngine;

public class PlatformController {

    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly CarryingSystem carryingSystem;
    private readonly VehicleSystem vehicleSystem;
    private readonly HitboxSystem hitboxSystem;
    private readonly EntityMapping entityMapping;
    private readonly PoseRegistry poseRegistry;
    private readonly PlatformPresenter presenter;

    private int idCounter;
    private readonly Dictionary<int, PlatformModel> registry = new();

    public PlatformController(HealthSystem healthSystem, FactionRegistry factionRegistry, CarryingSystem carryingSystem, VehicleSystem vehicleSystem, PlatformPresenter presenter, HitboxSystem hitboxSystem, EntityMapping entityMapping, PoseRegistry poseRegistry) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.carryingSystem = carryingSystem;
        this.vehicleSystem = vehicleSystem;
        this.presenter = presenter;
        this.hitboxSystem = hitboxSystem;
        this.entityMapping = entityMapping;
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
        model.CarryingId = carryingSystem.Add(prototype.carryingPrototype, model.PoseId);
        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        model.VehicleId = vehicleSystem.Create(initPosition, prototype.vehiclePrototype, poseId: model.PoseId);
        model.HitboxId = hitboxSystem.Add(model.FactionId, model.PoseId, prototype.hitboxPrototype);

        entityMapping.CreateMapping(model.HitboxId, new EntityComponents {
            healthId = model.HealthId,
            factionId = model.FactionId
        });
        
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
            healthId: platform.HealthId,
            healthState: healthSystem.ReadState(platform.HealthId),
            factionState: factionRegistry.ReadState(platform.FactionId),
            vehicleId: platform.VehicleId,
            weaponId: carryingState.weaponId,
            weaponState: carryingState.weaponState,
            platformId: platform.Id
        );
    }

    private List<int> removalBuffer = new();

    private void RemoveDestroyed() {
        removalBuffer.Clear();
        foreach (var platform in registry.Values) {
            var healthState = healthSystem.ReadState(platform.HealthId);
            if (healthState.isDead)
                removalBuffer.Add(platform.Id);
        }

        foreach (var destroyed in removalBuffer) {
            Remove(destroyed);
        }
    }

    private void Remove(int platformId) {
        registry.Remove(platformId, out var model);
        carryingSystem.Remove(model.CarryingId);
        healthSystem.Remove(model.HealthId);
        vehicleSystem.Remove(model.VehicleId);
        poseRegistry.Remove(model.PoseId);
        hitboxSystem.Remove(model.HitboxId);
        entityMapping.DeleteMapping(model.HitboxId);
        factionRegistry.Remove(model.FactionId);
        
        presenter.Remove(platformId);
    }

    private void UpdatePresentation() {
        foreach (var host in registry.Values) {
            var vehicleState = vehicleSystem.ReadState(host.VehicleId);
            var healthState = healthSystem.ReadState(host.HealthId);
            presenter.Update(host.Id, vehicleState, healthState);
        }
    }

}
