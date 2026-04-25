using UnityEngine;

public struct ProductionBuildingPrototype {
    public Vector3 position;
    public Quaternion rotation;
    public GameObject visualsPrefab;
    public PhysicsObstacle dimensionsPrefab;
    public ProductionBuildingConfig config;
    public SpawnSpot spawnSpot;
}