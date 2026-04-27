using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class SpawnService {
    
    private readonly InfantryController infantryController;
    private readonly ArmorController armorController;

    private readonly List<Vector3> spawnPointsBuffer = new(32);
    private readonly List<int> idsBuffer = new(32);

    public SpawnService(InfantryController infantryController, ArmorController armorController) {
        this.infantryController = infantryController;
        this.armorController = armorController;
    }

    public int GetProductionLoad(SpawnType spawnType) {
        switch (spawnType) {
            case SpawnType.Infantry: 
                return infantryController.InfantryCount;
            case SpawnType.Armor:
                return armorController.ArmorCount;
            default:
                Debug.LogError($"{spawnType}");
                return int.MaxValue;
        }
    }

    public SpawnResult Spawn(SpawnSpot spot, SpawnVariant variant, int limit) {
        idsBuffer.Clear();
        spot.shape.CalculateSpawnPoints(spawnPointsBuffer);
        
        foreach (var spawnPoint in spawnPointsBuffer.Take(limit)) {
            var worldSpaceSpawnPoint = spot.position + spot.rotation * spawnPoint;
            
            if (variant.type == SpawnType.Infantry) {
                var prototype = variant.infantryPrototype;
                prototype.position = worldSpaceSpawnPoint;
                var spawnedId = infantryController.SpawnInfantry(prototype);
                idsBuffer.Add(spawnedId);
            } else if (variant.type == SpawnType.Armor) {
                var prototype = variant.armorPrototype;
                prototype.position = worldSpaceSpawnPoint;
                var spawnedId = armorController.SpawnArmor(prototype);
                idsBuffer.Add(spawnedId);
            }
        }

        return new SpawnResult {
            spawnType = variant.type,
            spawnedIds = idsBuffer.ToArray(),
        };
    }

}