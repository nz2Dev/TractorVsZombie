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

    public SpawnResult Spawn(SpawnRequest request) {
        idsBuffer.Clear();
        request.shape.CalculateSpawnPoints(spawnPointsBuffer);
        
        foreach (var spawnPoint in spawnPointsBuffer.Take(request.amount)) {
            var worldSpaceSpawnPoint = request.position + request.rotation * spawnPoint;
            
            if (request.spawnType == SpawnType.Infantry) {
                var spawnedId = infantryController.SpawnInfantry(worldSpaceSpawnPoint, request.alie, request.spawnConfig.infantryConfig);
                idsBuffer.Add(spawnedId);
            } else if (request.spawnType == SpawnType.Armor) {
                var spawnedId = armorController.SpawnArmor(worldSpaceSpawnPoint, request.spawnConfig.armorConfig);
                idsBuffer.Add(spawnedId);
            }
        }

        return new SpawnResult {
            spawnType = request.spawnType,
            spawnedIds = idsBuffer.ToArray(),
        };
    }

}