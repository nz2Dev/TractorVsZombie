using UnityEngine;

public enum SpawnType {
    Infantry,
    Armor    
}

[CreateAssetMenu(fileName = "Spawn Config", menuName = "SpawnConfig", order = 0)]
public class SpawnConfig : ScriptableObject {
    public float interval;
    public SpawnType spawnType;
    public InfantryConfig infantryConfig;
    public ArmorConfig armorConfig;
}