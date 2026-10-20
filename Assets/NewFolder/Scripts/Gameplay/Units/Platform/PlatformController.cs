using System;
using System.Collections.Generic;


using UnityEngine;

public class PlatformController {

    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly LoadoutController loadoutController;
    private readonly VehicleSystem vehicleSystem;
    private readonly HitboxSystem hitboxSystem;
    private readonly EntityMapping entityMapping;
    private readonly PlatformView view;

    private int idCounter;
    private readonly Dictionary<int, PlatformModel> registry = new();

    public PlatformController(HealthSystem healthSystem, FactionRegistry factionRegistry, LoadoutController loadoutController, VehicleSystem vehicleSystem, PlatformView view, HitboxSystem hitboxSystem, EntityMapping entityMapping) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.loadoutController = loadoutController;
        this.vehicleSystem = vehicleSystem;
        this.view = view;
        this.hitboxSystem = hitboxSystem;
        this.entityMapping = entityMapping;
    }

    public void Update() {
        ReadHealth();
        SyncPositions();
        UpdateView();
        RemoveDestroyed();
    }

    public virtual int Create(PlatformPrototype prototype, Vector3 position = default) {
        var nextId = ++idCounter;
        var initPosition = position == default ? prototype.position : position;
        var model = new PlatformModel(nextId, initPosition, prototype.loadoutOffset);
        registry[model.Id] = model;

        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        model.VehicleId = vehicleSystem.Create(model.Position, prototype.vehiclePrefab);
        model.HitboxId = hitboxSystem.Add(model.FactionId, initPosition, prototype.hitboxPrototype);

        entityMapping.CreateMapping(model.HitboxId, new EntityComponents {
            healthId = model.HealthId,
            factionId = model.FactionId
        });
        
        view.AddPlatform(model.Id, model.Position, prototype.visualsPrefab, factionRegistry.ReadState(model.FactionId).alie, prototype.worldSpaceUIPrefab);
        return model.Id;
    }

    internal bool Exist(int platformId) {
        return registry.ContainsKey(platformId);
    }

    public virtual void SetLoadout(int platformId, LoadoutPrototype loadoutPrototype) {
        var platform = registry[platformId];

        if (platform.LoadoutId != 0) {
            loadoutController.DeleteLoadout(platform.LoadoutId);
        }

        loadoutPrototype.position = platform.Position + platform.LoadoutOffset;
        platform.LoadoutId = loadoutController.SpawnLoadout(platform.FactionId, loadoutPrototype);
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
        var loadoutState = default (LoadoutState);
        if (platform.LoadoutId != 0) {
            loadoutState = loadoutController.ReadLoadoutState(platform.LoadoutId);
        }
        return new PlatformState (
            position: platform.Position,
            healthId: platform.HealthId,
            healthState: healthSystem.ReadState(platform.HealthId),
            factionState: factionRegistry.ReadState(platform.FactionId),
            vehicleId: platform.VehicleId,
            weaponId: loadoutState.weaponId,
            weaponState: loadoutState.weaponState,
            platformId: platform.Id
        );
    }

    private void ReadHealth() {
        foreach (var platform in registry.Values) {
            platform.HealthState = healthSystem.ReadState(platform.HealthId);
            if (platform.HealthState.damageResult?.damageWasFatal == true) {
                if (platform.LoadoutId != 0) {
                    loadoutController.DeleteLoadout(platform.LoadoutId);
                    platform.LoadoutId = 0;
                }
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
        healthSystem.Remove(model.HealthId);
        vehicleSystem.Remove(model.VehicleId);
        hitboxSystem.Remove(model.HitboxId);
        entityMapping.DeleteMapping(model.HitboxId);
        factionRegistry.Remove(model.FactionId);
        
        view.RemovePlatform(platformId);
    }

    private void SyncPositions() {
        foreach (var host in registry.Values) {
            var vehicleState = vehicleSystem.ReadState(host.VehicleId);
            host.Position = vehicleState.position;
            view.UpdatePlatformPose(host.Id, vehicleState);

            if (host.LoadoutId != 0) {
                loadoutController.MoveLoadout(host.LoadoutId, host.Position + host.LoadoutOffset, vehicleState.rotation);
            }

            hitboxSystem.UpdatePosition(host.HitboxId, host.Position);
        }
    }

    private void UpdateView() {
        foreach (var host in registry.Values) {
            var healthState = host.HealthState;
            view.UpdateHealthBar(host.Id, host.Position, healthState.health, healthState.maxHealth);
            
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
