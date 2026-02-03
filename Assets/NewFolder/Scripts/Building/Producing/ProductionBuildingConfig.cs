using System;

using UnityEngine;

[Serializable]
public enum SpawnType {
    Infantry,
    Armor
}

[CreateAssetMenu(fileName = "Production Building Config", menuName = "ProductionBuildingConfig", order = 0)]
public class ProductionBuildingConfig : ScriptableObject {
    public int maxHealth = 100;
    public float height = 2f;
    public float radius = 1f;
    public GameObject visualsPrefab;
    public Vector3 vehicleObstacleSize = new Vector3(3, 2, 3);
    public Vector3 physicsObstacleSize = new Vector3(3, 2, 3);
    [Space]
    public float spawnInterval = 5f;
    public SpawnType spawnType;
    public InfantryConfig infantryConfig;
    public ArmorConfig armorConfig;
}
