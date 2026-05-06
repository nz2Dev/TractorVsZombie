
using System;
using System.Collections.Generic;

using UnityEngine;

public class HeadquarterBuildingController {
    
    private readonly CombatSystem combatSystem;
    private readonly PathfindingService pathfindingService;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly VehicleService vehicleService;
    private readonly PhysicsService physicsService;

    private GameObject visuals;
    private HeadquarterBuilding headquarter;

    public HeadquarterBuildingController(CombatSystem combatSystem, PathfindingService pathfindingService, VehicleService vehicleService, PhysicsService physicsService, LocalAvoidanceService localAvoidanceService) {
        this.combatSystem = combatSystem;
        this.pathfindingService = pathfindingService;
        this.vehicleService = vehicleService;
        this.physicsService = physicsService;
        this.localAvoidanceService = localAvoidanceService;
    }

    public void Update() {
        ReadCombatOutput();
        CheckLooseCondition();
    }

    public void Create(HeadquarterBuildingPrototype prototype) {
        headquarter = new HeadquarterBuilding(prototype.config);
        headquarter.Position = prototype.position;
        headquarter.CombatId = combatSystem.RegisterAgent(prototype.position, prototype.config.alie, prototype.config.maxHealth, height: 2);
        headquarter.PathfindingObstacleId = pathfindingService.RegisterObstacle(prototype.position, prototype.config.radius);
        headquarter.AvoidanceObstacleId = localAvoidanceService.AddObstacle(prototype.position, prototype.rotation, prototype.config.avoidanceObstaclePrefab);
        headquarter.VehicleObstacleId = vehicleService.RegisterObstacle(prototype.position, prototype.config.vehicleObstaclePrefab);
        headquarter.PhysicsObstacleId = physicsService.RegisterObstacle(prototype.position, prototype.config.physicsObstaclePrefab);
        visuals = GameObject.Instantiate(prototype.config.visualsPrefab, prototype.position, prototype.rotation);
    }

    private void ReadCombatOutput() {
        var combatOutput = combatSystem.GetCombatOutput(headquarter.CombatId);
        if (combatOutput.damageWasFatal) {
            headquarter.Destroyed = true;
            GameObject.Destroy(visuals);
            pathfindingService.UnregisterObstacle(headquarter.PathfindingObstacleId);
            localAvoidanceService.RemoveObstacle(headquarter.AvoidanceObstacleId);
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