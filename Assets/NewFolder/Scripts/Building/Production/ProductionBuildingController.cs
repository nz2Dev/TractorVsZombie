using System;
using System.Collections.Generic;
using System.Linq;

using Compatibility;

using UnityEngine;
using UnityEngine.Assertions;

public class ProductionBuildingController {

    private readonly ProductionBuildingView view;
    private readonly CombatSystem combatSystem;
    private readonly VehicleService vehicleService;
    private readonly RagdollService physicsService;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly SpawnService spawnService;

    private int idCounter;
    private readonly Dictionary<int, ProductionBuildingModel> registry = new();
    private readonly Dictionary<int, int> uniqueIdRegistry = new();
    private readonly List<int> removalBuffer = new();

    public ProductionBuildingController(
        ProductionBuildingView view,
        CombatSystem combatSystem,
        VehicleService vehicleService,
        RagdollService physicsService,
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

    public int RegisterUniqueId(int uniqueId) {
        Assert.IsFalse(uniqueId == 0);
        int nextId;
        if (uniqueIdRegistry.ContainsKey(uniqueId)) {
            nextId = uniqueIdRegistry[uniqueId];
        } else {
            nextId = ++idCounter;
            uniqueIdRegistry[uniqueId] = nextId;
        }
        return nextId;
    }

    public int Create(ProductionBuildingPrototype prototype) {
        int nextId;
        if (prototype.uniqueId == 0) {
            nextId = ++idCounter;
        } else if (!uniqueIdRegistry.ContainsKey(prototype.uniqueId)) {
            nextId = ++idCounter;
            uniqueIdRegistry[prototype.uniqueId] = nextId;
        } else {
            nextId = uniqueIdRegistry[prototype.uniqueId];
        }

        var model = new ProductionBuildingModel(nextId, prototype.config, prototype.spawnSpot, prototype.spawnVariant);
        model.Position = prototype.position;
        model.Rotation = prototype.rotation;
        model.QueueAmount = prototype.config.initialQueueAmount;
        model.NextSpawnTime = Time.time;

        model.CombatId = combatSystem.RegisterAgent(model.Position, prototype.combatAgentPrototype);
        model.AvoidanceObstacleId = localAvoidanceService.AddObstacle(model.Position, model.Rotation, prototype.dimensionsPrefab);
        model.VehicleObstacleId = vehicleService.RegisterObstacle(model.Position, prototype.dimensionsPrefab);
        model.PhysicsObstacleId = physicsService.RegisterObstacle(model.Position, prototype.physicsObstaclePrefab);
        registry[nextId] = model;

        view.AddVisuals(model.Id, model.Position, model.Rotation, prototype.visualsPrefab);
        return nextId;
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
            var spawnResult = spawnService.Spawn(model.SpawnSpot, model.SpawnVariant, availableSpawn);
            model.QueueAmount -= spawnResult.spawnedIds.Length;
            model.SpawnResult = spawnResult;
        }
    }

}
