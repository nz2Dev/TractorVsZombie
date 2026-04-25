using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ProductionBuildingController {

    private readonly ProductionBuildingView view;
    private readonly CombatSystem combatSystem;
    private readonly VehicleService vehicleService;
    private readonly PhysicsService physicsService;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly SpawnService spawnService;
    
    private int idCounter;
    private readonly Dictionary<int, ProductionBuildingModel> registry = new();
    private readonly List<int> removalBuffer = new();

    public ProductionBuildingController(
        ProductionBuildingView view,
        CombatSystem combatSystem,
        VehicleService vehicleService,
        PhysicsService physicsService,
        LocalAvoidanceService localAvoidanceService,
        SpawnService spawnService) {
        this.view = view;
        this.combatSystem = combatSystem;
        this.vehicleService = vehicleService;
        this.physicsService = physicsService;
        this.localAvoidanceService = localAvoidanceService;
        this.spawnService = spawnService;
    }

    public void Update() {
        ClearEvents();
        ReadCombatOutput();
        ValidateBuildings();
        ProduceSpawns();
    }

    public bool IsExist(int buildingId) {
        return registry.ContainsKey(buildingId);
    }

    public int Create(ProductionBuildingPrototype prototype) {
        var id = ++idCounter;
        var model = new ProductionBuildingModel(id, prototype.config, prototype.spawnSpot);
        
        model.Position = prototype.position;
        model.Rotation = prototype.rotation;
        model.QueueAmount = prototype.config.initialQueueAmount;
        model.NextSpawnTime = Time.time;

        model.CombatId = combatSystem.RegisterAgent(model.Position, prototype.config.alie, model.Config.maxHealth, prototype.config.height);
        model.AvoidanceObstacleId = localAvoidanceService.AddObstacle(model.Position, model.Rotation, prototype.dimensionsPrefab);
        model.VehicleObstacleId = vehicleService.RegisterObstacle(model.Position, prototype.dimensionsPrefab);
        model.PhysicsObstacleId = physicsService.RegisterObstacle(model.Position, prototype.dimensionsPrefab);
        registry[id] = model;

        view.AddVisuals(model.Id, model.Position, model.Rotation, prototype.visualsPrefab);
        return id;
    }

    public ProductionBuildingState ReadState(int buildingId) {
        var model = registry[buildingId];
        return new ProductionBuildingState {
            lastResult = model.SpawnResult
        };
    }

    private void ClearEvents() {
        foreach (var building in registry.Values) {
            building.SpawnResult = null;
        }
    }

    private void DestroyBuilding(int id) {
        registry.Remove(id, out var model);
        combatSystem.UnregisterAgent(model.CombatId);
        localAvoidanceService.RemoveObstacle(model.AvoidanceObstacleId);
        vehicleService.UnregisterObstacle(model.VehicleObstacleId);
        physicsService.UnregisterObstacle(model.PhysicsObstacleId);
        view.RemoveVisuals(model.Id);
    }

    private void ReadCombatOutput() {
        foreach (var model in registry.Values) {
            var combatOutput = combatSystem.GetCombatOutput(model.CombatId);
            if (combatOutput.damageWasFatal) {
                model.Destroyed = true;
            }
        }
    }

    private void ValidateBuildings() {
        removalBuffer.Clear();

        foreach (var model in registry.Values)
            if (model.Destroyed)
                removalBuffer.Add(model.Id);

        foreach (var id in removalBuffer)
            DestroyBuilding(id);
    }

    private void ProduceSpawns() {
        foreach (var model in registry.Values) {
            if (model.QueueAmount <= 0 || Time.time < model.NextSpawnTime)
                continue;

            var availableSpawn = model.QueueAmount;
            model.NextSpawnTime = Time.time + model.Config.spawnInterval;
            var spawnResult = spawnService.Spawn(model.SpawnSpot, availableSpawn);
            model.QueueAmount -= spawnResult.spawnedIds.Length;
            model.SpawnResult = spawnResult;
        }
    }

}
