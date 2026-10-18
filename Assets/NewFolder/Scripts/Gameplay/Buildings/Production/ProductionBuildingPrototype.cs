
using UnityEngine;

public struct ProductionBuildingPrototype {
    public int uniqueId;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject visualsPrefab;
    public ORCAObstacleVertices avoidanceObstaclePrefab;
    public CollisionObstacle collisionObstaclePrefab;
    public ProductionBuildingConfig config;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public SpawnerPrototype spawnerPrototype;

    public ProductionBuildingPrototype(int uniqueId, Vector3 position, Quaternion rotation, GameObject visualsPrefab, ORCAObstacleVertices avoidanceObstaclePrefab, CollisionObstacle physicsObstaclePrefab, ProductionBuildingConfig config, HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype, SpawnerPrototype spawnerPrototype) {
        this.uniqueId = uniqueId;
        this.position = position;
        this.rotation = rotation;
        this.visualsPrefab = visualsPrefab;
        this.avoidanceObstaclePrefab = avoidanceObstaclePrefab;
        this.collisionObstaclePrefab = physicsObstaclePrefab;
        this.config = config;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
        this.spawnerPrototype = spawnerPrototype;
    }
}
