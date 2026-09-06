using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class SpawnerController {
    
    private readonly InfantryController infantryController;
    private readonly ArmorController armorController;

    private readonly List<Vector3> spawnPointsBuffer = new(32);

    private int idCounter;
    private readonly Dictionary<SpawnerId, SpawnerModel> registry = new();

    public SpawnerController(InfantryController infantryController, ArmorController armorController) {
        this.infantryController = infantryController;
        this.armorController = armorController;
    }

    public SpawnerId Create(SpawnerPrototype prototype) {
        var nextId = new SpawnerId(++idCounter);
        var model = new SpawnerModel(nextId, prototype.config, prototype.spawnSpot, prototype.spawnVariant);
        model.Queue = model.Config.initialQueue;
        model.NextSpawnTime = Time.time;
        registry[nextId] = model;
        return nextId;
    }

    public void Destroy(SpawnerId spawnerId) {
        registry.Remove(spawnerId, out var model);
    }

    public SpawnResult GetLastSpawnResult(SpawnerId spawnerId) {
        return registry[spawnerId].LastSpawnEvent;
    }

    public void Update() {
        foreach (var model in registry.Values) {
            model.LastSpawnEvent = null;
            if (model.Queue <= 0 || Time.time < model.NextSpawnTime)
                continue;

            model.NextSpawnTime = Time.time + model.Config.spawnInterval;
            var availableSpawn = model.Queue;
            Spawn(model, model.SpawnSpot, model.SpawnVariant, availableSpawn);
            model.Queue -= model.LastSpawnEvent.spawnedIds.Length;
        }
    }

    private void Spawn(SpawnerModel model, SpawnSpot spot, SpawnVariant variant, int limit) {
        model.IdsBuffer.Clear();
        spot.shape.CalculateSpawnPoints(spawnPointsBuffer);
        
        foreach (var spawnPoint in spawnPointsBuffer.Take(limit)) {
            var worldSpaceSpawnPoint = spot.position + spot.rotation * spawnPoint;
            
            if (variant.type == SpawnVariantType.Infantry) {
                var prototype = variant.infantryPrototype;
                prototype.position = worldSpaceSpawnPoint;
                var spawnedId = infantryController.SpawnInfantry(prototype);
                model.IdsBuffer.Add(spawnedId);
            } else if (variant.type == SpawnVariantType.Armor) {
                var prototype = variant.armorPrototype;
                prototype.position = worldSpaceSpawnPoint;
                var spawnedId = armorController.SpawnArmor(prototype);
                model.IdsBuffer.Add(spawnedId);
            }
        }

        model.LastSpawnEvent = new SpawnResult {
            spawnType = variant.type,
            spawnedIds = model.IdsBuffer.ToArray(),
        };
    }

}