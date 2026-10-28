using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class SpawnerController {
    
    private readonly SpawnService spawnService;

    private int idCounter;
    private readonly Dictionary<SpawnerId, SpawnerModel> registry = new();

    public SpawnerController(SpawnService spawnService) {
        this.spawnService = spawnService;
    }

    public SpawnerId Create(SpawnerPrototype prototype) {
        var nextId = new SpawnerId(++idCounter);
        var model = new SpawnerModel(nextId);
        ResetSpawner(model, prototype.spawnPrototype, prototype.config);
        registry[nextId] = model;
        return nextId;
    }

    public void Configure(SpawnerId spawnerId, SpawnPrototype prototype, SpawnerConfig config) {
        ResetSpawner(registry[spawnerId], prototype, config);
    }

    private void ResetSpawner(SpawnerModel model, SpawnPrototype prototype, SpawnerConfig config) {
        model.SpawnCount = 0;
        model.NextSpawnTime = Time.time;
        model.SpawnPrototype = prototype;
        model.Config = config;
    }

    public void Destroy(SpawnerId spawnerId) {
        registry.Remove(spawnerId);
    }

    public void Update() {
        foreach (var model in registry.Values) {
            model.LastSpawnEvent = null;
            if (model.SpawnCount >= model.Config.times || Time.time < model.NextSpawnTime)
                continue;

            model.LastSpawnEvent = spawnService.Spawn(model.SpawnPrototype);
            model.NextSpawnTime = Time.time + model.Config.interval;
            model.SpawnCount++;;
        }
    }

    public SpawnResult GetLastSpawnResult(SpawnerId spawnerId) {
        return registry[spawnerId].LastSpawnEvent;
    }

}