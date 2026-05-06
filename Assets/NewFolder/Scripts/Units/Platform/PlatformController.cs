using System;
using System.Collections.Generic;

using UnityEngine;

public class PlatformController {

    private readonly CombatSystem combatSystem;
    private readonly LoadoutController loadoutController;
    private readonly RamEffectController ramEffect;
    private readonly VehicleService vehicleService;
    private readonly PlatformView view;

    private int idCounter;
    private readonly Dictionary<int, PlatformModel> registry = new();

    public PlatformController(CombatSystem combatSystem, LoadoutController loadoutController, RamEffectController ramEffect, VehicleService vehicleService, PlatformView view) {
        this.combatSystem = combatSystem;
        this.loadoutController = loadoutController;
        this.ramEffect = ramEffect;
        this.vehicleService = vehicleService;
        this.view = view;
    }

    public void Update() {
        SyncPositions();
    }
    
    public int Create(PlatformPrototype prototype, Vector3 position = default) {
        var nextId = ++idCounter;
        var initPosition = position == default ? prototype.position : position;
        var model = new PlatformModel(nextId, initPosition, prototype.config);
        registry[model.Id] = model;
        
        model.LoadoutOffset = prototype.loadoutOffset;
        model.CombatId = combatSystem.RegisterAgent(model.Position, true);
        model.VehiclePhysicsId = vehicleService.CreateVehicle(model.Position, prototype.physicsPrefab);
        model.RamId = ramEffect.StartNew(model.CombatId, prototype.ramPrototype);
        view.AddPlatform(model.Id, model.Position, prototype.visualsPrefab);

        return model.Id;
    }

    public void Connect(int tailPlatformId, int headVehiclePhysicsId) {
        var tailPlatform = registry[tailPlatformId];
        var headState = vehicleService.GetVehicleState(headVehiclePhysicsId);
        
        var towardHeadRotation = Quaternion.LookRotation((headState.position - tailPlatform.Position).normalized, Vector3.up);
        vehicleService.UpdateVehiclePose(tailPlatform.VehiclePhysicsId, tailPlatform.Position, towardHeadRotation);
        vehicleService.MakeTowingConnection(headVehiclePhysicsId, tailPlatform.VehiclePhysicsId);
    }

    public void Disconnect(int platformId) {
        var platform = registry[platformId];
        vehicleService.ClearTowingConnection(platform.VehiclePhysicsId);
    }

    public void SetLoadout(int platformId, LoadoutPrototype loadoutPrototype) {
        var platform = registry[platformId];
        
        if (platform.LoadoutId != 0) {
            loadoutController.DeleteLoadout(platform.LoadoutId);
        }

        loadoutPrototype.position = platform.Position + platform.LoadoutOffset;
        platform.LoadoutId = loadoutController.SpawnLoadout(platform.CombatId, loadoutPrototype);
    }

    public int GetVehiclePhysicsId(int platformId) {
        return registry[platformId].VehiclePhysicsId;
    }

    public PlatformState ReadPlatformState(int platformId) {
        var platform = registry[platformId];
        var loadoutState = default (LoadoutState);
        if (platform.LoadoutId != 0) {
            loadoutState = loadoutController.ReadLoadoutState(platform.LoadoutId);
        }
        return new PlatformState {
            position = platform.Position,
            combatId = platform.CombatId,
            vehiclePhysicsId = platform.VehiclePhysicsId,
            weaponId = loadoutState.weaponId,
            weaponState = loadoutState.weaponState,
            platformId = platform.Id
        };
    }

    private void SyncPositions() {
        foreach (var host in registry.Values) {
            host.VehiclePhysicsState = vehicleService.GetVehicleState(host.VehiclePhysicsId);
            host.Position = host.VehiclePhysicsState.position;
            view.UpdatePlatformPose(host.Id, host.VehiclePhysicsState);
            
            if (host.LoadoutId != 0) {
                loadoutController.MoveLoadout(host.LoadoutId, host.Position + host.LoadoutOffset, host.VehiclePhysicsState.rotation);
            }

            combatSystem.UpdateAgentPosition(host.CombatId, host.Position);
            ramEffect.Forward(host.RamId, host.Position);
        }
    }

}