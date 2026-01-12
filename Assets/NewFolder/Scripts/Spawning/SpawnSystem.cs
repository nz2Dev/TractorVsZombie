using System;
using System.Collections.Generic;

using UnityEngine;

public class SpawnSystem {

    private readonly int maxArmorCount;
    private readonly ArmorController armorController;
    private readonly int maxInfantryCount;
    private readonly InfantryController infantryController;

    private int idCounter;
    private readonly Dictionary<int, SpawnModel> registry = new();
    
    public SpawnSystem(InfantryController infantryController, ArmorController armorController, int maxArmorCount, int maxInfantryCount) {
        this.infantryController = infantryController;
        this.armorController = armorController;
        this.maxArmorCount = maxArmorCount;
        this.maxInfantryCount = maxInfantryCount;
    }

    public int AddSpawner(Vector3 position, SpawnConfig spawnConfig) {
        var nextId = ++idCounter;
        var model = new SpawnModel(nextId, spawnConfig, position);
        registry[nextId] = model;
        return nextId;
    }

    public void CaptureCompletedSpawns(IEnumerable<int> spawnerIds, List<SpawnResult> output) {
        output.Clear();
        foreach (var spawnerId in spawnerIds) {
            var spawner = registry[spawnerId];
            foreach (var entityId in spawner.SpawnedIds) {
                output.Add(new SpawnResult {
                    spawnType = spawner.SpawnConfig.spawnType,
                    spawnedId = entityId,
                });
            }
        }
    }

    public void Update() {
        ClearSpawnResults();
        ProduceSpawns();
    }

    private void ClearSpawnResults() {
        foreach (var model in registry.Values) {
            model.SpawnedIds.Clear();
        }
    }

    private void ProduceSpawns() {
        foreach (var model in registry.Values) {
            if (model.LastSpawnTime + model.SpawnConfig.interval > Time.time)
                continue;

            model.LastSpawnTime = Time.time;
            var spawnPoint = model.Position + Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, Vector3.up);
            
            if (model.SpawnConfig.spawnType == SpawnType.Infantry) {
                if (infantryController.InfantryCount > maxInfantryCount)
                    continue;

                var infantryId = infantryController.SpawnInfantry(spawnPoint, alie: false, model.SpawnConfig.infantryConfig);
                model.SpawnedIds.Add(infantryId);
            } else if (model.SpawnConfig.spawnType == SpawnType.Armor) {
                if (armorController.ArmorCount > maxArmorCount)
                    continue;
                    
                var armorId = armorController.SpawnArmor(spawnPoint, model.SpawnConfig.armorConfig);
                model.SpawnedIds.Add(armorId);
            } else {
                throw new Exception();
            }
        }
    }

}