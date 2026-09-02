using Combat;

using UnityEngine;

public struct ProductionBuildingPrototype {
    public int uniqueId;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject visualsPrefab;
    public PhysicsObstacle dimensionsPrefab;
    public CollisionObstacle collisionObstaclePrefab;
    public ProductionBuildingConfig config;
    public SpawnSpot spawnSpot;
    public SpawnVariant spawnVariant;
    public CombatPrototype combatPrototype;
    public RaycastMarker raycastMarkerPrefab;

    public ProductionBuildingPrototype(int uniqueId, Vector3 position, Quaternion rotation, GameObject visualsPrefab, PhysicsObstacle dimensionsPrefab, CollisionObstacle physicsObstaclePrefab, ProductionBuildingConfig config, SpawnSpot spawnSpot, SpawnVariant spawnVariant, CombatPrototype combatPrototype, RaycastMarker raycastMarkerPrefab) {
        this.uniqueId = uniqueId;
        this.position = position;
        this.rotation = rotation;
        this.visualsPrefab = visualsPrefab;
        this.dimensionsPrefab = dimensionsPrefab;
        this.collisionObstaclePrefab = physicsObstaclePrefab;
        this.config = config;
        this.spawnSpot = spawnSpot;
        this.spawnVariant = spawnVariant;
        this.combatPrototype = combatPrototype;
        this.raycastMarkerPrefab = raycastMarkerPrefab;
    }
}
