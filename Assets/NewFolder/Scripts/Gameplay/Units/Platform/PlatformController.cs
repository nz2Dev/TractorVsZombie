using System;
using System.Collections.Generic;

using Combat;

using UnityEngine;

public class PlatformController {

    private readonly CombatSystem combatSystem;
    private readonly LoadoutController loadoutController;
    private readonly VehicleSystem vehicleSystem;
    private readonly ProximityService proximityService;
    private readonly RaycastService raycastService;
    private readonly EntityMapping entityMapping;
    private readonly PlatformView view;

    private int idCounter;
    private readonly Dictionary<int, PlatformModel> registry = new();

    public PlatformController(CombatSystem combatSystem, LoadoutController loadoutController, VehicleSystem vehicleSystem, PlatformView view, ProximityService proximityService, RaycastService raycastService, EntityMapping entityMapping) {
        this.combatSystem = combatSystem;
        this.loadoutController = loadoutController;
        this.vehicleSystem = vehicleSystem;
        this.view = view;
        this.proximityService = proximityService;
        this.raycastService = raycastService;
        this.entityMapping = entityMapping;
    }

    public void Update() {
        ReadCombat();
        SyncPositions();
        UpdateView();
        RemoveDestroyed();
    }

    public virtual int Create(PlatformPrototype prototype, Vector3 position = default) {
        var nextId = ++idCounter;
        var initPosition = position == default ? prototype.position : position;
        var model = new PlatformModel(nextId, initPosition, prototype.config, prototype.loadoutOffset);
        registry[model.Id] = model;

        model.CombatId = combatSystem.Add(prototype.combatPrototype);
        model.VehicleId = vehicleSystem.Create(model.Position, prototype.vehiclePrefab);
        model.ProximityId = proximityService.AddPoint(initPosition, CombatSystem.GetProximityLayerForFaction(prototype.combatPrototype.alie));
        model.RaycastId = raycastService.RegisterMarker(initPosition, prototype.raycastMarkerPrefab, CombatSystem.GetRaycastLayerForFaction(prototype.combatPrototype.alie));

        entityMapping.CreateMappings(new EntityComponents {
            proximityId = model.ProximityId,
            raycastId = model.RaycastId,
            combatId = model.CombatId
        });
        
        view.AddPlatform(model.Id, model.Position, prototype.visualsPrefab, prototype.combatPrototype.alie, prototype.worldSpaceUIPrefab);
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
        platform.LoadoutId = loadoutController.SpawnLoadout(platform.CombatId, loadoutPrototype);
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
            combatId: platform.CombatId,
            combatState: combatSystem.ReadState(platform.CombatId),
            vehicleId: platform.VehicleId,
            weaponId: loadoutState.weaponId,
            weaponState: loadoutState.weaponState,
            platformId: platform.Id
        );
    }

    private void ReadCombat() {
        foreach (var platform in registry.Values) {
            platform.CombatState = combatSystem.ReadState(platform.CombatId);
            if (platform.CombatState.damageResult?.damageWasFatal == true) {
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
            if (platform.CombatState.isDead)
                removalBuffer.Add(platform.Id);
        }

        foreach (var destroyed in removalBuffer) {
            Remove(destroyed);
        }
    }

    private void Remove(int platformId) {
        registry.Remove(platformId, out var model);
        combatSystem.Remove(model.CombatId);
        vehicleSystem.Remove(model.VehicleId);
        proximityService.RemovePoint(model.ProximityId);
        raycastService.UnregisterMarker(model.RaycastId);
        entityMapping.DeleteMappings(model.ProximityId, model.RaycastId);
        
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

            proximityService.UpdatePoint(host.ProximityId, host.Position);
            raycastService.UpdateMarker(host.RaycastId, host.Position);
        }
    }

    private void UpdateView() {
        foreach (var host in registry.Values) {
            var combatState = host.CombatState;
            view.UpdateHealthBar(host.Id, host.Position, combatState.health, combatState.maxHealth);
            
            if (combatState.damageResult.HasValue) {
                // interesting case occure, conceptually platform is only a carrier
                // loadout and weapons are handled separately
                // but ideally when hit occures, all three has to show take hit effect.
                // so either all of them separatly check for provided ownerCombatId state and trigger visual feedback separatly
                // or should the visual component of entier platform entity handle all of the visuals for loadout and weapon? (first option I like more)
                view.ShowTakeHit(host.Id);
            }
        }
    }

}
