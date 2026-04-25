using UnityEngine;

public class ProductionBuildingSource : MonoBehaviour {

    [SerializeField] private ProductionBuildingConfig config;
    [SerializeField] private SpawnSpotSource spawnSpotSource;
    [SerializeField] private PhysicsObstacle dimensionsPrefab;
    [SerializeField] private GameObject visualsPrefab;

    public ProductionBuildingPrototype GetPrototype() {
        return new ProductionBuildingPrototype {
            position = transform.position,
            rotation = transform.rotation,
            config = config,
            spawnSpot = spawnSpotSource.Provide(),
            dimensionsPrefab = dimensionsPrefab,
            visualsPrefab = visualsPrefab,
        };
    }
}