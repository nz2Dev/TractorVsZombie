
using System;
using System.Collections.Generic;

using UnityEngine;

public class HeadquarterBuildingController {
    
    private readonly CombatSystem combatSystem;
    private readonly PathfindingService pathfindingService;
    private readonly VehicleService vehicleService;
    private readonly PhysicsService physicsService;

    private GameObject visuals;
    private HeadquarterBuilding headquarter;

    public HeadquarterBuildingController(CombatSystem combatSystem, PathfindingService pathfindingService, VehicleService vehicleService, PhysicsService physicsService) {
        this.combatSystem = combatSystem;
        this.pathfindingService = pathfindingService;
        this.vehicleService = vehicleService;
        this.physicsService = physicsService;
    }

    public void Update() {
        ReadCombatOutput();
        CheckLooseCondition();
    }

    public void SetHeadquearter(Vector3 position, Quaternion rotation, HeadquarterBuildingConfig config) {
        headquarter = new HeadquarterBuilding(config);
        headquarter.Position = position;
        headquarter.CombatId = combatSystem.RegisterAgent(position, config.alie, config.maxHealth, height: 2);
        headquarter.ObstacleId = pathfindingService.RegisterObstacle(position, config.radius);
        headquarter.VehicleObstacleId = vehicleService.RegisterObstacle(position, config.VehicleObstacleSize);
        headquarter.PhysicsObstacleId = physicsService.RegisterObstacle(position, config.physicsObstacleSize);
        visuals = GameObject.Instantiate(config.visualsPrefab, position, rotation);
    }

    private void ReadCombatOutput() {
        var combatOutput = combatSystem.GetCombatOutput(headquarter.CombatId);
        if (combatOutput.damageWasFatal) {
            headquarter.Destroyed = true;
            GameObject.Destroy(visuals);
            pathfindingService.UnregisterObstacle(headquarter.ObstacleId);
            vehicleService.UnregisterObstacle(headquarter.VehicleObstacleId);
            physicsService.UnregisterObstacle(headquarter.PhysicsObstacleId);
        }
    }

    private void CheckLooseCondition() {
        if (headquarter.Destroyed) {
            Debug.Log("Game over");
        }
    }

}