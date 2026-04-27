using System;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

public class ProductionSpaceController {
    
    private readonly SpawnService spawnService;

    public ProductionSpaceController(SpawnService spawnService) {
        this.spawnService = spawnService;
    }

    private int idCounter;
    private readonly Dictionary<int, ProductionSpaceModel> registry = new ();
    private readonly Dictionary<int, int> uniqueIdRegistry = new();

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

    public int Create(ProductionSpacePrototype prototype) {
        int nextId;
        if (prototype.uniqueId == 0) {
            nextId = ++idCounter;
        } else if (!uniqueIdRegistry.ContainsKey(prototype.uniqueId)) {
            nextId = ++idCounter;
            uniqueIdRegistry[prototype.uniqueId] = nextId;
        } else {
            nextId = uniqueIdRegistry[prototype.uniqueId];
        }

        var model = new ProductionSpaceModel(nextId, prototype.config, prototype.spawnSpot, prototype.spawnVariant);
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
            var result = spawnService.Spawn(model.SpawnSpot, model.SpawnVariant, availableSpawn);
            model.Queue -= result.spawnedIds.Length;
            model.LastSpawnEvent = result;
        }
    }

}