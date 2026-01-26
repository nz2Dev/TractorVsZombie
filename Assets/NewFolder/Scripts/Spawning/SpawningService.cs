using UnityEngine;

public class SpawningService {

    private readonly int maxArmorCount;
    private readonly ArmorController armorController;
    private readonly int maxInfantryCount;
    private readonly InfantryController infantryController;

    public SpawningService(InfantryController infantryController, ArmorController armorController, int maxArmorCount, int maxInfantryCount) {
        this.infantryController = infantryController;
        this.armorController = armorController;
        this.maxArmorCount = maxArmorCount;
        this.maxInfantryCount = maxInfantryCount;
    }

    public bool TryProduceInfantry(Vector3 sourcePosition, bool alie, InfantryConfig config, out int spawnedId) {
        spawnedId = -1;
        if (infantryController.InfantryCount >= maxInfantryCount)
            return false;

        var spawnPoint = sourcePosition + Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, Vector3.up);
        spawnedId = infantryController.SpawnInfantry(spawnPoint, alie, config);
        return true;
    }

    public bool TryProduceArmor(Vector3 sourcePosition, ArmorConfig config, out int spawnedId) {
        spawnedId = -1;
        if (armorController.ArmorCount >= maxArmorCount)
            return false;
                
        var spawnPoint = sourcePosition + Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, Vector3.up);
        spawnedId = armorController.SpawnArmor(spawnPoint, config);
        return true;
    }

}
