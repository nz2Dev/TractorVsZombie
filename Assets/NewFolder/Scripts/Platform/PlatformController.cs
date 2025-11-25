using System;
using System.Collections.Generic;

using UnityEngine;

public class PlatformController {

    private readonly CombatService combatService;
    private readonly VehicleController vehicleController;
    private readonly WeaponController weaponController;

    private int idCounter;
    private readonly Dictionary<int, PlatformModel> registry = new();

    public PlatformController(CombatService combatService, VehicleController vehicleController, WeaponController weaponController) {
        this.combatService = combatService;
        this.vehicleController = vehicleController;
        this.weaponController = weaponController;
    }

    public void Update() {
        SyncPositions();
    }
    
    public int SpawnPlatform(Vector3 position, PlatformConfig config, int headVehicleId) {
        var nextId = ++idCounter;
        var platform = new PlatformModel(nextId, position, config);
        registry[platform.Id] = platform;
        
        platform.CombatId = combatService.RegisterAgent(position, alie: true);
        platform.VehicleId = vehicleController.SpawnVehicle(position, platform.CombatId, platform.VehicleConfig);
        vehicleController.ConnectVehicles(headVehicleId, platform.VehicleId);

        return platform.Id;
    }

    public PlatformState ReadPlatformState(int platformId) {
        var platform = registry[platformId];
        return new PlatformState {
            position = platform.Position,
            combatId = platform.CombatId,
            vehicleId = platform.VehicleId,
            weaponId = platform.WeaponId,
            weaponConfig = platform.WeaponConfig
        };
    }

    public void SetWeapon(int platformId, WeaponConfig weaponConfig) {
        var platform = registry[platformId];
        platform.WeaponConfig = weaponConfig;
        platform.WeaponId = weaponController.SpawnWeapon(platform.CombatId, platform.Position, weaponConfig);
    }

    private void SyncPositions() {
        foreach (var host in registry.Values) {
            host.Position = vehicleController.GetVehiclePosition(host.VehicleId);
            weaponController.MoveWeapon(host.WeaponId, host.Position);
            combatService.UpdateAgentPosition(host.CombatId, host.Position);
        }
    }

}