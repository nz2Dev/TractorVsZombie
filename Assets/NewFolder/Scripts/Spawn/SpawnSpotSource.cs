using UnityEngine;

public class SpawnSpotSource : MonoBehaviour {
    
    [SerializeField] private SpawnType type;
    [SerializeField] private SpawnShape shape;
    [SerializeField] private InfantrySource infantrySource;
    [SerializeField] private ArmorSource armorSource;
    [SerializeField] private SpawnConfig config;

    public SpawnSpot Provide() {
        var spotConfig = config;
        if (type == SpawnType.Infantry && infantrySource != null) {
            spotConfig.infantryPrototype = infantrySource.GetPrototype();
        } else if (type == SpawnType.Armor && armorSource != null) {
            spotConfig.armorPrototype = armorSource.GetPrototype();
        }
        return new SpawnSpot {
            position = transform.position,
            rotation = transform.rotation,
            config = spotConfig,
            shape = shape,
            type = type
        };
    }

}