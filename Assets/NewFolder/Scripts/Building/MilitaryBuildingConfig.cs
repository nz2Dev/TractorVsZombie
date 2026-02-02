using System;

using UnityEngine;

[Serializable]
public enum SpawnType {
    Infantry,
    Armor
}

[CreateAssetMenu(fileName = "Military Building Config", menuName = "MilitaryBuildingConfig", order = 0)]
public class MilitaryBuildingConfig : ScriptableObject {
    public int maxHealth = 100;
    public float height = 2f;
    public float radius = 1f;
    public GameObject visualsPrefab;
    [Space]
    public float spawnInterval = 5f;
    public SpawnType spawnType;
    public InfantryConfig infantryConfig;
    public ArmorConfig armorConfig;
}
