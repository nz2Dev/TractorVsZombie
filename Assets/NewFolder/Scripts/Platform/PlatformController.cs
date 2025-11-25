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
        OperateWeapons();
    }
    
    public int SpawnPlatform(Vector3 position, PlatformConfig config, int headVehicleId) {
        var nextId = ++idCounter;
        var platform = new PlatformModel(nextId, position, config);
        registry[platform.Id] = platform;
        
        platform.CombatId = combatService.RegisterAgent(position, alie: true);
        platform.VehicleId = vehicleController.SpawnVehicle(position, platform.VehicleConfig);
        vehicleController.ConnectVehicles(headVehicleId, platform.VehicleId);

        return platform.Id;
    }

    public int GetPlatformVehicleId(int platformId) {
        return registry[platformId].VehicleId;
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

    private void OperateWeapons() {
        foreach (var platform in registry.Values) {
            var enemySearchRadius = platform.WeaponConfig.aimConfig.range;
            if (combatService.GetClosestEnemyAgentInRange(platform.CombatId, enemySearchRadius, out var agentInfo)) {
                weaponController.AimWeapon(platform.WeaponId, agentInfo.position + 0.5f * agentInfo.height * Vector3.up);
            }
        }
    }

}