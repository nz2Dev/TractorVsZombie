using UnityEngine;

public class ProductionSpaceSource : MonoBehaviour {
    
    [SerializeField] private ProductionSpaceConfig config;
    [SerializeField] private SpawnSpotSource spawnSpotSource;

    public ProductionSpacePrototype GetPrototype() {
        return new ProductionSpacePrototype {
            config = config,
            position = transform.position,
            rotation = transform.rotation,
            spawnSpot = spawnSpotSource.Provide(), 
        };
    }
}