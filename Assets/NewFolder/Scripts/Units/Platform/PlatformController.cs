using System;
using System.Collections.Generic;

using Combat;

using UnityEngine;

public class PlatformController {

    private readonly CombatSystem combatSystem;
    private readonly LoadoutController loadoutController;
    private readonly RamEffectController ramEffect;
    private readonly VehicleService vehicleService;
    private readonly ProximityService proximityService;
    private readonly RaycastService raycastService;
    private readonly EntityMapping entityMapping;
    private readonly PlatformView view;

    private int idCounter;
    private readonly Dictionary<int, PlatformModel> registry = new();

    public PlatformController(CombatSystem combatSystem, LoadoutController loadoutController, RamEffectController ramEffect, VehicleService vehicleService, PlatformView view, ProximityService proximityService, RaycastService raycastService, EntityMapping entityMapping) {
        this.combatSystem = combatSystem;
        this.loadoutController = loadoutController;
        this.ramEffect = ramEffect;
        this.vehicleService = vehicleService;
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
        model.VehiclePhysicsId = vehicleService.CreateVehicle(model.Position, prototype.vehiclePrefab);
        model.ProximityId = proximityService.AddPoint(initPosition, CombatSystem.GetProximityLayerForFaction(prototype.combatPrototype.alie));
        model.RaycastId = raycastService.RegisterMarker(initPosition, prototype.raycastMarkerPrefab, CombatSystem.GetRaycastLayerForFaction(prototype.combatPrototype.alie));

        model.RamId = ramEffect.StartNew(model.CombatId, model.VehiclePhysicsId, prototype.combatPrototype.alie, prototype.ramPrototype);

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

    public virtual void Connect(int tailPlatformId, int headVehiclePhysicsId) {
        var tailPlatform = registry[tailPlatformId];
        var headState = vehicleService.GetVehicleState(headVehiclePhysicsId);

        var towardHeadRotation = Quaternion.LookRotation((headState.position - tailPlatform.Position).normalized, Vector3.up);
        vehicleService.UpdateVehiclePose(tailPlatform.VehiclePhysicsId, tailPlatform.Position, towardHeadRotation);
        vehicleService.MakeTowingConnection(headVehiclePhysicsId, tailPlatform.VehiclePhysicsId);
    }

    public virtual void Disconnect(int platformId) {
        var platform = registry[platformId];
        vehicleService.ClearTowingConnection(platform.VehiclePhysicsId);
    }

    public virtual void SetLoadout(int platformId, LoadoutPrototype loadoutPrototype) {
        var platform = registry[platformId];

        if (platform.LoadoutId != 0) {
            loadoutController.DeleteLoadout(platform.LoadoutId);
        }

        loadoutPrototype.position = platform.Position + platform.LoadoutOffset;
        platform.LoadoutId = loadoutController.SpawnLoadout(platform.CombatId, loadoutPrototype);
    }

    public virtual int GetVehiclePhysicsId(int platformId) {
        return registry[platformId].VehiclePhysicsId;
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
            vehiclePhysicsId: platform.VehiclePhysicsId,
            weaponId: loadoutState.weaponId,
            weaponState: loadoutState.weaponState,
            platformId: platform.Id
        );
    }

    private void ReadCombat() {
        foreach (var platform in registry.Values) {
            var combatState = combatSystem.ReadState(platform.CombatId);
            if (combatState.health <= 0) {
                platform.Destroyed = true;

                // should vehicle library handle disconnection itself?
                // we do this, so that we don't have to do this in caller sites
                // it's naturall to assume that if platform is destroyed, it removes its connections
                // but it also naturall to assume that if we delete the vehicle from service it will remove it
                // currently vehicle implementation don't destroy game object and its configurable joints, which should effectively remove physics constraints
                // todo: consider moving this implicit behavior handling to vehicle implementation
                vehicleService.ClearTowingConnection(platform.VehiclePhysicsId);
                
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
            if (platform.Destroyed)
                removalBuffer.Add(platform.Id);
        }

        foreach (var destroyed in removalBuffer) {
            Remove(destroyed);
        }
    }

    private void Remove(int platformId) {
        registry.Remove(platformId, out var model);
        combatSystem.Remove(model.CombatId);
        vehicleService.DeleteVehicle(model.VehiclePhysicsId);
        proximityService.RemovePoint(model.ProximityId);
        raycastService.UnregisterMarker(model.RaycastId);
        ramEffect.Remove(model.RamId);
        entityMapping.DeleteMappings(model.ProximityId, model.RaycastId);
        
        view.RemovePlatform(platformId);
    }

    private void SyncPositions() {
        foreach (var host in registry.Values) {
            host.VehiclePhysicsState = vehicleService.GetVehicleState(host.VehiclePhysicsId);
            host.Position = host.VehiclePhysicsState.position;
            view.UpdatePlatformPose(host.Id, host.VehiclePhysicsState);

            if (host.LoadoutId != 0) {
                loadoutController.MoveLoadout(host.LoadoutId, host.Position + host.LoadoutOffset, host.VehiclePhysicsState.rotation);
            }

            proximityService.UpdatePoint(host.ProximityId, host.Position);
            raycastService.UpdateMarker(host.RaycastId, host.Position);
            ramEffect.Forward(host.RamId, host.Position);
        }
    }

    private void UpdateView() {
        foreach (var host in registry.Values) {
            var combatState = combatSystem.ReadState(host.CombatId);
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
