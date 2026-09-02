using System.Collections.Generic;

using Combat;

using UnityEditor;

using UnityEngine;
using UnityEngine.Assertions;

public class ProductionBuildingController {

    private readonly ProductionBuildingView view;
    private readonly CombatSystem combatSystem;
    private readonly CollisionService collisionService;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly SpawnService spawnService;
    private readonly ProximityService proximityService;
    private readonly RaycastService raycastService;
    private readonly EntityMapping entityMapping;

    private int idCounter;
    private readonly Dictionary<int, ProductionBuildingModel> registry = new();
    private readonly Dictionary<int, int> uniqueIdRegistry = new();
    private readonly List<int> removalBuffer = new();

    public ProductionBuildingController(
        ProductionBuildingView view,
        CombatSystem combatSystem,
        CollisionService collisionService,
        LocalAvoidanceService localAvoidanceService,
        SpawnService spawnService,
        ProximityService proximityService,
        RaycastService raycastService,
        EntityMapping entityMapping) {
        this.view = view;
        this.combatSystem = combatSystem;
        this.collisionService = collisionService;
        this.localAvoidanceService = localAvoidanceService;
        this.spawnService = spawnService;
        this.proximityService = proximityService;
        this.raycastService = raycastService;
        this.entityMapping = entityMapping;
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
        registry[nextId] = model;
        model.Position = prototype.position;
        model.Rotation = prototype.rotation;
        model.QueueAmount = prototype.config.initialQueueAmount;
        model.NextSpawnTime = Time.time;

        model.CombatId = combatSystem.Add(prototype.combatPrototype);
        model.AvoidanceObstacleId = localAvoidanceService.AddObstacle(model.Position, model.Rotation, prototype.dimensionsPrefab);
        model.CollisionObstacleId = collisionService.RegisterObstacle(model.Position, prototype.collisionObstaclePrefab);
        model.ProximityId = proximityService.AddPoint(prototype.position, CombatSystem.GetProximityLayerForFaction(prototype.combatPrototype.alie));
        model.RaycastId = raycastService.RegisterMarker(prototype.position, prototype.raycastMarkerPrefab, CombatSystem.GetRaycastLayerForFaction(prototype.combatPrototype.alie));

        entityMapping.CreateMappings(new EntityComponents {
            proximityId = model.ProximityId,
            raycastId = model.RaycastId,
            combatId = model.CombatId
        });

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
        
        combatSystem.Remove(model.CombatId);
        localAvoidanceService.RemoveObstacle(model.AvoidanceObstacleId);
        collisionService.UnregisterObstacle(model.CollisionObstacleId);
        raycastService.UnregisterMarker(model.RaycastId);
        proximityService.RemovePoint(model.ProximityId);

        entityMapping.DeleteMappings(model.ProximityId, model.RaycastId);

        view.RemoveVisuals(model.Id);
    }

    private void ReadCombatOutput() {
        foreach (var model in registry.Values) {
            var combatState = combatSystem.ReadState(model.CombatId);
            if (combatState.damageResult?.damageWasFatal == true) {
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
