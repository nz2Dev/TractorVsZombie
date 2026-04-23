using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ProductionBuildingController {

    private readonly ProductionBuildingView view;
    private readonly CombatSystem combatSystem;
    private readonly PathfindingService pathfindingService;
    private readonly VehicleService vehicleService;
    private readonly PhysicsService physicsService;
    private readonly LocalAvoidanceService localAvoidanceService;
    
    private int idCounter;
    private readonly Dictionary<int, ProductionBuildingModel> registry = new();
    private readonly List<int> removalBuffer = new();

    public ProductionBuildingController(
        ProductionBuildingView view,
        CombatSystem combatSystem,
        PathfindingService pathfindingService,
        VehicleService vehicleService,
        PhysicsService physicsService,
        LocalAvoidanceService localAvoidanceService) {
        this.view = view;
        this.combatSystem = combatSystem;
        this.pathfindingService = pathfindingService;
        this.vehicleService = vehicleService;
        this.physicsService = physicsService;
        this.localAvoidanceService = localAvoidanceService;
    }

    public void Update() {
        ReadCombatOutput();
        ValidateBuildings();
        ProduceSpawns();
    }

    public bool IsExist(int buildingId) {
        return registry.ContainsKey(buildingId);
    }

    public int Spawn(Vector3 position, Quaternion rotation, ProductionBuildingConfig config, bool alie) {
        var id = ++idCounter;
        var model = new ProductionBuildingModel(id, config);
        
        model.Alie = alie;
        model.Position = position;
        model.Rotation = rotation;
        model.CombatId = combatSystem.RegisterAgent(position, alie, model.Config.maxHealth, config.height);
        // model.PathfindingObstacleId = pathfindingService.RegisterObstacle(position, (int)config.radius);
        model.AvoidanceObstacleId = localAvoidanceService.AddObstacle(position, rotation, config.avoidanceObstaclePrefab);
        model.VehicleObstacleId = vehicleService.RegisterObstacle(position, config.vehicleObstaclePrefab);
        model.PhysicsObstacleId = physicsService.RegisterObstacle(position, config.physicsObstaclePrefab);
        model.NextSpawnTime = Time.time;
        model.QueueAmount = config.initialQueueAmount;
        registry[id] = model;

        view.AddVisuals(model.Id, position, rotation, model.Config.visualsPrefab);
        return id;
    }

    public void SetQueueAmount(int buildingId, int amount) {
        registry[buildingId].QueueAmount = amount;
    }

    public bool TryReadLastSpawnRequest(int buildingId, out SpawnRequest request) {
        var model = registry[buildingId];
        request = model.LastRequest;
        var wasRequested = model.SpawnRequested;
        model.SpawnRequested = false;
        return wasRequested;
    }

    private void DestroyBuilding(int id) {
        registry.Remove(id, out var model);
        combatSystem.UnregisterAgent(model.CombatId);
        // pathfindingService.UnregisterObstacle(model.PathfindingObstacleId);
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

            var availableSpawn = Mathf.Min(model.QueueAmount, model.Config.spawnShapePrefab.GetTotalPoints());
            model.NextSpawnTime = Time.time + model.Config.spawnInterval;
            model.QueueAmount -= availableSpawn;
            model.SpawnRequested = true;
            model.LastRequest = new SpawnRequest {
                amount = availableSpawn,
                spawnType = model.Config.spawnType,
                shape = model.Config.spawnShapePrefab,
                spawnPoint = new SpawnPoint {
                    position = model.Position,
                    rotation = model.Rotation,
                },
                alie = model.Alie,
                spawnConfig = model.Config.spawnConfig
            };
        }
    }

}
