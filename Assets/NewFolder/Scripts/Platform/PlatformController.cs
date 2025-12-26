using System;
using System.Collections.Generic;

using UnityEngine;

public class PlatformController {

    private readonly CombatService combatService;
    private readonly TowableVehicleController towableVehicleController;
    private readonly WeaponController weaponController;

    private int idCounter;
    private readonly Dictionary<int, PlatformModel> registry = new();

    public PlatformController(CombatService combatService, TowableVehicleController towableVehicleController, WeaponController weaponController) {
        this.combatService = combatService;
        this.towableVehicleController = towableVehicleController;
        this.weaponController = weaponController;
    }

    public void Update() {
        SyncPositions();
    }
    
    public int SpawnPlatform(Vector3 position, PlatformConfig config) {
        var nextId = ++idCounter;
        var platform = new PlatformModel(nextId, position, config);
        registry[platform.Id] = platform;
        
        platform.CombatId = combatService.RegisterAgent(position, alie: true);
        platform.VehicleId = towableVehicleController.SpawnVehicle(position, platform.CombatId, platform.VehicleConfig);
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
            host.Position = towableVehicleController.GetVehiclePosition(host.VehicleId);
            weaponController.MoveWeapon(host.WeaponId, host.Position);
            combatService.UpdateAgentPosition(host.CombatId, host.Position);
        }
    }

}