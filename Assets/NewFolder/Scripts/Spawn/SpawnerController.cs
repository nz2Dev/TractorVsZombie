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
        ResetSpawner(model, prototype.initSetup);
        registry[nextId] = model;
        return nextId;
    }

    public void Configure(SpawnerId spawnerId, SpawnSetup setup) {
        ResetSpawner(registry[spawnerId], setup);
    }

    private void ResetSpawner(SpawnerModel model, SpawnSetup setup) {
        model.SpawnCount = 0;
        model.NextSpawnTime = Time.time;
        model.Setup = setup;
    }

    public void Destroy(SpawnerId spawnerId) {
        registry.Remove(spawnerId);
    }

    public void Update() {
        foreach (var model in registry.Values) {
            model.LastSpawnEvent = null;
            if (model.SpawnCount >= model.Setup.config.times || Time.time < model.NextSpawnTime)
                continue;

            Spawn(model, model.Setup.spot, model.Setup.variant);
            model.NextSpawnTime = Time.time + model.Setup.config.interval;
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