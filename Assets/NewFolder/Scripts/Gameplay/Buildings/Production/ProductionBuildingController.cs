using System.Collections.Generic;


using UnityEditor;

using UnityEngine;
using UnityEngine.Assertions;

public class ProductionBuildingController {

    private readonly ProductionBuildingView view;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly CollisionService collisionService;
    private readonly AvoidanceService localAvoidanceService;
    private readonly SpawnerController spawnerController;
    private readonly HitboxSystem hitboxSystem;
    private readonly EntityMapping entityMapping;

    private int idCounter;
    private readonly Dictionary<int, ProductionBuildingModel> registry = new();
    private readonly Dictionary<int, int> uniqueIdRegistry = new();
    private readonly List<int> removalBuffer = new();

    public ProductionBuildingController(
        ProductionBuildingView view,
        HealthSystem healthSystem,
        FactionRegistry factionRegistry,
        CollisionService collisionService,
        AvoidanceService localAvoidanceService,
        SpawnerController spawnService,
        HitboxSystem hitboxSystem,
        EntityMapping entityMapping) {
        this.view = view;
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.collisionService = collisionService;
        this.localAvoidanceService = localAvoidanceService;
        this.spawnerController = spawnService;
        this.hitboxSystem = hitboxSystem;
        this.entityMapping = entityMapping;
    }

    public void Update() {
        ReadHealthOutput();
        ValidateBuildings();
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

        var model = new ProductionBuildingModel(nextId);
        registry[nextId] = model;
        
        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        model.AvoidanceObstacleId = localAvoidanceService.AddObstacle(prototype.avoidanceObstaclePrefab);
        model.CollisionObstacleId = collisionService.RegisterObstacle(prototype.position, prototype.collisionObstaclePrefab);
        model.HitboxId = hitboxSystem.Add(model.FactionId, prototype.position, prototype.hitboxPrototype);
        model.SpawnerId = spawnerController.Create(prototype.spawnerPrototype);

        entityMapping.CreateMapping(model.HitboxId, new EntityComponents {
            healthId = model.HealthId,
            factionId = model.FactionId
        });

        view.AddVisuals(model.Id, prototype.position, prototype.rotation, prototype.visualsPrefab);
        return nextId;
    }

    public ProductionBuildingState ReadState(int buildingId) {
        var model = registry[buildingId];
        return new ProductionBuildingState {
            spawnerId = model.SpawnerId,
            lastResult = spawnerController.GetLastSpawnResult(model.SpawnerId)
        };
    }

    private void DestroyBuilding(int id) {
        registry.Remove(id, out var model);
        
        healthSystem.Remove(model.HealthId);
        localAvoidanceService.RemoveObstacle(model.AvoidanceObstacleId);
        collisionService.UnregisterObstacle(model.CollisionObstacleId);
        hitboxSystem.Remove(model.HitboxId);
        spawnerController.Destroy(model.SpawnerId);

        entityMapping.DeleteMapping(model.HitboxId);
        factionRegistry.Remove(model.FactionId);

        view.RemoveVisuals(model.Id);
    }

    private void ReadHealthOutput() {
        foreach (var model in registry.Values) {
            var healthState = healthSystem.ReadState(model.HealthId);
            if (healthState.damageResult?.damageWasFatal == true) {
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

}
