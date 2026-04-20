using System.Collections.Generic;

using UnityEngine;

public class WaveController {

    private readonly SpawnService spawnService;

    public WaveController(SpawnService spawnService) {
        this.spawnService = spawnService;
    }

    private int idCounter;
    private readonly Dictionary<int, WaveModel> registry = new();

    public int Create(WaveConfig waveConfig) {
        var nextId = idCounter++;
        var model = new WaveModel(nextId, waveConfig);
        model.Queue = waveConfig.initialQueue;
        model.NextSpawnTime = Time.time;
        registry[nextId] = model;
        return nextId;
    }

    public void Update() {
        foreach (var model in registry.Values) {
            if (model.Queue <= 0 || Time.time < model.NextSpawnTime)
                continue;

            var availableSpawn = Mathf.Min(model.Queue, model.Config.spawnShape.GetTotalPoints());
            model.NextSpawnTime = Time.time + model.Config.spawnInterval;
            model.Queue -= availableSpawn;
            model.SpawnResult = spawnService.Spawn(new SpawnRequest {
                alie = false,
                amount = availableSpawn,
                position = model.Config.worldPointPrefab.position,
                rotation = model.Config.worldPointPrefab.rotation,
                shape = model.Config.spawnShape,
                spawnConfig = model.Config.spawnConfig,
                spawnType = model.Config.spawnType
            });
        }
    }
}