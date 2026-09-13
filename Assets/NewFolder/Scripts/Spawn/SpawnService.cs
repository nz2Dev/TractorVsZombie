using System.Collections.Generic;

using UnityEngine;

public class SpawnService {
    
    private readonly InfantryController infantryController;
    private readonly ArmorController armorController;

    private readonly List<Vector3> spawnPointsBuffer = new (32);
    private readonly List<int> idsBuffer = new (32);

    public SpawnService(InfantryController infantryController, ArmorController armorController) {
        this.infantryController = infantryController;
        this.armorController = armorController;
    }

    public SpawnResult Spawn(SpawnPrototype prototype) {
        idsBuffer.Clear();
        prototype.shape.CalculateSpawnPoints(spawnPointsBuffer);
        
        foreach (var spawnPoint in spawnPointsBuffer) { // todo limit logic was removed, consider restoring
            var worldSpaceSpawnPoint = prototype.position + prototype.rotation * spawnPoint;
            
            if (prototype.variant.type == SpawnVariantType.Infantry) {
                var infantryPrototype = prototype.variant.infantryPrototype;
                infantryPrototype.position = worldSpaceSpawnPoint;
                var spawnedId = infantryController.SpawnInfantry(infantryPrototype);
                idsBuffer.Add(spawnedId);
            } else if (prototype.variant.type == SpawnVariantType.Armor) {
                var armorPrototype = prototype.variant.armorPrototype;
                armorPrototype.position = worldSpaceSpawnPoint;
                var spawnedId = armorController.SpawnArmor(armorPrototype);
                idsBuffer.Add(spawnedId);
            }
        }

        return new SpawnResult {
            spawnType = prototype.variant.type,
            spawnedIds = idsBuffer.ToArray(),
        };
    }
}