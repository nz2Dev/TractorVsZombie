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
    private readonly PlatformView view;

    private int idCounter;
    private readonly Dictionary<int, PlatformModel> registry = new();

    public PlatformController(HealthSystem healthSystem, FactionRegistry factionRegistry, CarryingSystem carryingSystem, VehicleSystem vehicleSystem, PlatformView view, HitboxSystem hitboxSystem, EntityMapping entityMapping, PoseRegistry poseRegistry) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.carryingSystem = carryingSystem;
        this.vehicleSystem = vehicleSystem;
        this.view = view;
        this.hitboxSystem = hitboxSystem;
        this.entityMapping = entityMapping;
        this.poseRegistry = poseRegistry;
    }

    public void Update() {
        ReadHealth();
        UpdateView();
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
        
        view.AddPlatform(model.Id, initPosition, prototype.visualsPrefab, factionRegistry.ReadState(model.FactionId).alie, prototype.worldSpaceUIPrefab);
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

    private void ReadHealth() {
        foreach (var platform in registry.Values) {
            platform.HealthState = healthSystem.ReadState(platform.HealthId);
            if (platform.HealthState.damageResult?.damageWasFatal == true) {
                carryingSystem.RemoveLoadout(platform.CarryingId);
            }
        }
    }

    private List<int> removalBuffer = new();

    private void RemoveDestroyed() {
        removalBuffer.Clear();
        foreach (var platform in registry.Values) {
            if (platform.HealthState.isDead)
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
        
        view.RemovePlatform(platformId);
    }

    private void UpdateView() {
        foreach (var host in registry.Values) {
            var vehiclePose = vehicleSystem.ReadState(host.VehicleId).pose;
            view.UpdatePlatformPose(host.Id, vehiclePose);
            
            var healthState = host.HealthState;
            view.UpdateHealthBar(host.Id, vehiclePose.position, healthState.health, healthState.maxHealth);
            if (healthState.damageResult.HasValue) {
                // interesting case occure, conceptually platform is only a carrier
                // loadout and weapons are handled separately
                // but ideally when hit occures, all three has to show take hit effect.
                // so either all of them separatly check for provided ownerHealthId state and trigger visual feedback separatly
                // or should the visual component of entier platform entity handle all of the visuals for loadout and weapon? (first option I like more)
                view.ShowTakeHit(host.Id);
            }
        }
    }

}
