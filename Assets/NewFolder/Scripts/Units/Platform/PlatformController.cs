using System;
using System.Collections.Generic;

using UnityEngine;

public class PlatformController {

    private readonly CombatSystem combatSystem;
    private readonly WeaponController weaponController;
    private readonly RamEffect ramEffect;
    private readonly VehicleService vehicleService;
    private readonly PlatformView view;

    private int idCounter;
    private readonly Dictionary<int, PlatformModel> registry = new();

    public PlatformController(CombatSystem combatSystem, WeaponController weaponController, RamEffect ramEffect, VehicleService vehicleService, PlatformView view) {
        this.combatSystem = combatSystem;
        this.weaponController = weaponController;
        this.ramEffect = ramEffect;
        this.vehicleService = vehicleService;
        this.view = view;
    }

    public void Update() {
        SyncPositions();
    }
    
    public int SpawnPlatform(Vector3 position, PlatformConfig config) {
        var nextId = ++idCounter;
        var platform = new PlatformModel(nextId, position, config);
        registry[platform.Id] = platform;
        
        platform.CombatId = combatSystem.RegisterAgent(position, true);
        platform.VehiclePhysicsId = vehicleService.CreateVehicle(position, platform.Config.physicsPrefab);
        platform.RamId = ramEffect.StartNew(platform.CombatId, new RamPrototype {position = position, config = platform.Config.ramConfig});
        view.AddPlatform(platform.Id, position, platform.Config.visualsPrefab);

        return platform.Id;
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

    public void SetLoadout(int platformId, LoadoutConfig loadout) {
        var platform = registry[platformId];
        platform.WeaponConfig = loadout.weaponConfig;
        platform.WeaponPlacementOffset = loadout.weaponLocalOffset;
        platform.WeaponId = weaponController.SpawnWeapon(platform.CombatId, platform.Position, loadout.weaponConfig);
        view.SetLoadoutVisuals(platformId, loadout.brokenVisualsPrefab, platform.Config.LoadoutOffset);
    }

    public int GetVehiclePhysicsId(int platformId) {
        return registry[platformId].VehiclePhysicsId;
    }

    public PlatformState ReadPlatformState(int platformId) {
        var platform = registry[platformId];
        return new PlatformState {
            position = platform.Position,
            combatId = platform.CombatId,
            vehiclePhysicsId = platform.VehiclePhysicsId,
            weaponId = platform.WeaponId,
            weaponConfig = platform.WeaponConfig,
            platformId = platform.Id
        };
    }

    private void SyncPositions() {
        foreach (var host in registry.Values) {
            host.VehiclePhysicsState = vehicleService.GetVehicleState(host.VehiclePhysicsId);
            host.Position = host.VehiclePhysicsState.position;
            view.UpdatePlatformPose(host.Id, host.VehiclePhysicsState);

            weaponController.MoveWeapon(host.WeaponId, host.Position + host.Config.LoadoutOffset + host.WeaponPlacementOffset);
            combatSystem.UpdateAgentPosition(host.CombatId, host.Position);
            ramEffect.Forward(host.RamId, host.Position);
        }
    }

}