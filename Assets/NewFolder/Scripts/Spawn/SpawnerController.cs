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
        var model = new SpawnerModel(nextId);
        ResetSpawner(model, prototype.initConfig, prototype.initSpawnSpot, prototype.initSpawnVariant);
        registry[nextId] = model;
        return nextId;
    }

    public void Configure(SpawnerId spawnerId, SpawnConfig config, SpawnSpot spot, SpawnVariant variant) {
        ResetSpawner(registry[spawnerId], config, spot, variant);
    }

    private void ResetSpawner(SpawnerModel model, SpawnConfig config, SpawnSpot spot, SpawnVariant variant) {
        model.SpawnCount = 0;
        model.NextSpawnTime = Time.time;
        model.SpawnSpot = spot;
        model.SpawnVariant = variant;
        model.SpawnConfig = config;
    }

    public void Destroy(SpawnerId spawnerId) {
        registry.Remove(spawnerId);
    }

    public void Update() {
        foreach (var model in registry.Values) {
            model.LastSpawnEvent = null;
            if (model.SpawnCount >= model.SpawnConfig.times || Time.time < model.NextSpawnTime)
                continue;

            Spawn(model, model.SpawnSpot, model.SpawnVariant);
            model.NextSpawnTime = Time.time + model.SpawnConfig.interval;
            model.SpawnCount++;;
        }
    }

    private void Spawn(SpawnerModel model, SpawnSpot spot, SpawnVariant variant) {
        model.IdsBuffer.Clear();
        spot.shape.CalculateSpawnPoints(spawnPointsBuffer);
        
        foreach (var spawnPoint in spawnPointsBuffer) { // todo limit logic was removed, consider restoring
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

    public SpawnResult GetLastSpawnResult(SpawnerId spawnerId) {
        return registry[spawnerId].LastSpawnEvent;
    }

}