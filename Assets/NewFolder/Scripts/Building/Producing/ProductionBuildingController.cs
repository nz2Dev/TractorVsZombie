using System.Collections.Generic;

using UnityEngine;

public class ProductionBuildingController {

    private readonly ProductionBuildingView view;
    private readonly CombatSystem combatSystem;
    private readonly PathfindingService pathfindingService;
    private readonly VehicleService vehicleService;
    private readonly PhysicsService physicsService;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly InfantryController infantryController;
    private readonly ArmorController armorController;
    
    private int idCounter;
    private readonly Dictionary<int, ProductionBuildingModel> registry = new();
    private readonly List<int> removalBuffer = new();

    public ProductionBuildingController(
        ProductionBuildingView view,
        CombatSystem combatSystem,
        PathfindingService pathfindingService,
        VehicleService vehicleService,
        PhysicsService physicsService,
        LocalAvoidanceService localAvoidanceService,
        InfantryController infantryController,
        ArmorController armorController) {
        this.view = view;
        this.combatSystem = combatSystem;
        this.pathfindingService = pathfindingService;
        this.vehicleService = vehicleService;
        this.physicsService = physicsService;
        this.localAvoidanceService = localAvoidanceService;
        this.infantryController = infantryController;
        this.armorController = armorController;
    }

    public void Update() {
        ReadCombatOutput();
        ValidateBuildings();
        ProduceSpawns();
    }

    public bool IsExist(int buildingId) {
        return registry.ContainsKey(buildingId);
    }

    public int GetProductionLoad(SpawnType spawnType) {
        switch (spawnType) {
            case SpawnType.Infantry: 
                return infantryController.InfantryCount;
            case SpawnType.Armor:
                return armorController.ArmorCount;
            default:
                Debug.LogError($"{spawnType}");
                return int.MaxValue;
        }
    }

    public int Spawn(Vector3 position, Quaternion rotation, ProductionBuildingConfig config, bool alie) {
        var id = ++idCounter;
        var model = new ProductionBuildingModel(id, config);
        
        model.Alie = alie;
        model.Position = position;
        model.CombatId = combatSystem.RegisterAgent(position, alie, model.Config.maxHealth, config.height);
        // model.PathfindingObstacleId = pathfindingService.RegisterObstacle(position, (int)config.radius);
        model.AvoidanceObstacleId = localAvoidanceService.AddObstacle(position, rotation, config.avoidanceObstaclePrefab);
        model.VehicleObstacleId = vehicleService.RegisterObstacle(position, config.vehicleObstaclePrefab);
        model.PhysicsObstacleId = physicsService.RegisterObstacle(position, config.physicsObstaclePrefab);
        model.NextSpawnTime = Time.time + config.spawnInterval;
        model.QueueAmount = config.initialQueueAmount;
        registry[id] = model;

        view.AddVisuals(model.Id, position, rotation, model.Config.visualsPrefab);
        return id;
    }

    public void SetQueueAmount(int buildingId, int amount) {
        registry[buildingId].QueueAmount = amount;
    }

    public ProductionResult ReadProductionResult(int buildingId) {
        var model = registry[buildingId];
        return new ProductionResult {
            spawnType = model.Config.spawnType,
            spawnedIds = model.ProducedEntities.AsReadOnly()
        };
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
            model.ProducedEntities.Clear();
            if (model.QueueAmount <= 0 || Time.time < model.NextSpawnTime)
                continue;

            model.QueueAmount--;
            model.NextSpawnTime = Time.time + model.Config.spawnInterval;

            var spawnPoint = model.Position + Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, Vector3.up);
            if (model.Config.spawnType == SpawnType.Infantry) {
                var spawnedId = infantryController.SpawnInfantry(spawnPoint, model.Alie, model.Config.infantryConfig);
                model.ProducedEntities.Add(spawnedId);
            } else if (model.Config.spawnType == SpawnType.Armor) {
                var spawnedId = armorController.SpawnArmor(spawnPoint, model.Config.armorConfig);
                model.ProducedEntities.Add(spawnedId);
            }
        }
    }

}
