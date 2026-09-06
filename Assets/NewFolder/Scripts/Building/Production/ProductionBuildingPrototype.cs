using Combat;

using UnityEngine;

public struct ProductionBuildingPrototype {
    public int uniqueId;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject visualsPrefab;
    public ORCAObstacleVertices avoidanceObstaclePrefab;
    public CollisionObstacle collisionObstaclePrefab;
    public ProductionBuildingConfig config;
    public CombatPrototype combatPrototype;
    public RaycastMarker raycastMarkerPrefab;
    public SpawnerPrototype spawnerPrototype;

    public ProductionBuildingPrototype(int uniqueId, Vector3 position, Quaternion rotation, GameObject visualsPrefab, ORCAObstacleVertices avoidanceObstaclePrefab, CollisionObstacle physicsObstaclePrefab, ProductionBuildingConfig config, CombatPrototype combatPrototype, RaycastMarker raycastMarkerPrefab, SpawnerPrototype spawnerPrototype) {
        this.uniqueId = uniqueId;
        this.position = position;
        this.rotation = rotation;
        this.visualsPrefab = visualsPrefab;
        this.avoidanceObstaclePrefab = avoidanceObstaclePrefab;
        this.collisionObstaclePrefab = physicsObstaclePrefab;
        this.config = config;
        this.combatPrototype = combatPrototype;
        this.raycastMarkerPrefab = raycastMarkerPrefab;
        this.spawnerPrototype = spawnerPrototype;
    }
}
