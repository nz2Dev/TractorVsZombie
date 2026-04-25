using System;
using System.Collections.Generic;

using UnityEngine;

public class ProductionSpaceController {
    
    private readonly SpawnService spawnService;

    public ProductionSpaceController(SpawnService spawnService) {
        this.spawnService = spawnService;
    }

    private int idCounter;
    private readonly Dictionary<int, ProductionSpaceModel> registry = new ();

    public int Create(ProductionSpacePrototype prototype) {
        var nextId = ++idCounter;
        var model = new ProductionSpaceModel(nextId, prototype.config, prototype.spawnSpot);
        model.NextSpawnTime = Time.time;
        model.Queue = prototype.config.initialQueue;
        registry[nextId] = model;
        return nextId;
    }

    public bool IsExist(int productionSpaceId) {
        return registry.ContainsKey(productionSpaceId);
    }

    public SpawnResult ReadSpawnResult(int productionSpaceId) {
        return registry[productionSpaceId].LastSpawnEvent;
    }

    public void Update() {
        ClearSpawnEvents();
        ProduceSpawns();
    }

    private void ClearSpawnEvents() {
        foreach (var model in registry.Values) {
            model.LastSpawnEvent = null;
        }
    }

    private void ProduceSpawns() {
        foreach (var model in registry.Values) {
            if (model.Queue <= 0 || Time.time < model.NextSpawnTime)
                continue;

            model.NextSpawnTime = Time.time + model.Config.spawnInterval;
            var availableSpawn = model.Queue;
            var result = spawnService.Spawn(model.SpawnSpot, availableSpawn);
            model.Queue -= result.spawnedIds.Length;
            model.LastSpawnEvent = result;
        }
    }

}