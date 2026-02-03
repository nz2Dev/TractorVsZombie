using System.Collections.Generic;
using UnityEngine;

public class ProductionBuildingController {

    private readonly ProductionBuildingView view;
    private readonly CombatSystem combatSystem;
    private readonly SpawningService spawningService;
    private readonly BehaviorSystem behaviorSystem;
    private readonly CommanderSystem commanderSystem;
    private readonly ArmorAIController armorAIController;
    private readonly PathfindingService pathfindingService;
    private readonly VehicleService vehicleService;
    private readonly PhysicsService physicsService;
    private readonly LocalAvoidanceService localAvoidanceService;
    
    private int idCounter;
    private readonly Dictionary<int, ProductionBuildingModel> registry = new();
    private readonly List<int> removalBuffer = new();

    public ProductionBuildingController(
        CombatSystem combatSystem,
        SpawningService spawningService,
        BehaviorSystem behaviorSystem,
        CommanderSystem commanderSystem,
        ArmorAIController armorAIController,
        PathfindingService pathfindingService,
        VehicleService vehicleService,
        PhysicsService physicsService,
        ProductionBuildingView view,
        LocalAvoidanceService localAvoidanceService) {
        this.combatSystem = combatSystem;
        this.spawningService = spawningService;
        this.behaviorSystem = behaviorSystem;
        this.commanderSystem = commanderSystem;
        this.armorAIController = armorAIController;
        this.pathfindingService = pathfindingService;
        this.vehicleService = vehicleService;
        this.physicsService = physicsService;
        this.view = view;
        this.localAvoidanceService = localAvoidanceService;
    }

    public void Update() {
        ReadCombatOutput();
        ValidateBuildings();
        ProduceSpawns();
    }

    public int CreateBuilding(Vector3 position, Quaternion rotation, ProductionBuildingConfig config, bool alie, int commanderId) {
        var id = ++idCounter;
        var model = new ProductionBuildingModel(id, config);
        
        model.Position = position;
        model.Alie = alie;
        model.CommanderId = commanderId;
        model.CombatId = combatSystem.RegisterAgent(position, alie, model.Config.maxHealth, config.height);
        model.PathfindingObstacleId = pathfindingService.RegisterObstacle(position, (int)config.radius);
        model.AvoidanceObstacleId = localAvoidanceService.AddObstacle(position, rotation, config.avoidanceObstaclePrefab);
        model.VehicleObstacleId = vehicleService.RegisterObstacle(position, config.vehicleObstaclePrefab);
        model.PhysicsObstacleId = physicsService.RegisterObstacle(position, config.physicsObstaclePrefab);
        model.NextSpawnTime = Time.time + config.spawnInterval;
        registry[id] = model;

        view.AddVisuals(model.Id, position, rotation, model.Config.visualsPrefab);
        return id;
    }

    private void DestroyBuilding(int id) {
        registry.Remove(id, out var model);
        combatSystem.UnregisterAgent(model.CombatId);
        pathfindingService.UnregisterObstacle(model.PathfindingObstacleId);
        localAvoidanceService.RemoveObstacle(model.AvoidanceObstacleId);
        vehicleService.UnregisterObstacle(model.VehicleObstacleId);
        physicsService.UnregisterObstacle(model.PhysicsObstacleId);
        view.RemoveVisuals(model.Id);
    }

    public void SetAssignedCommander(int id, int commanderId) {
        registry[id].CommanderId = commanderId;
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
            if (Time.time < model.NextSpawnTime)
                continue;

            model.NextSpawnTime = Time.time + model.Config.spawnInterval;
            if (model.Config.spawnType == SpawnType.Infantry) {
                if (spawningService.TryProduceInfantry(model.Position, model.Alie, model.Config.infantryConfig, out var spawnedId)) {
                    var actorId = behaviorSystem.CreateActor(spawnedId);
                    commanderSystem.AddSubordinate(model.CommanderId, actorId);
                }
            } else if (model.Config.spawnType == SpawnType.Armor) {
                if (spawningService.TryProduceArmor(model.Position, model.Config.armorConfig, out var spawnedId)) {
                    armorAIController.AddAIBehaviour(spawnedId);
                }
            }
        }
    }

}
