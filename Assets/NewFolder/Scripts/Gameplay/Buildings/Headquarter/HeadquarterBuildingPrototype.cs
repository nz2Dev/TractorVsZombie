
using UnityEngine;

public struct HeadquarterBuildingPrototype {
    
    public Vector3 position;
    public Quaternion rotation;
    public HeadquarterBuildingConfig config;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public CollisionObstacle collisionObstaclePrefab;
    public ORCAObstacleVertices avoidanceObstaclePrefab;
    public Collider pathfindingObstaclePrefab;
    public HeadquarterBuildingVisuals visualsPrefab;
    public WorldSpaceUI worldSpaceUIPrefab;

    public HeadquarterBuildingPrototype(Vector3 position, Quaternion rotation, HeadquarterBuildingConfig config,
        HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype, CollisionObstacle collisionObstaclePrefab,
        ORCAObstacleVertices avoidanceObstaclePrefab, HeadquarterBuildingVisuals visualsPrefab, Collider pathfindingObstaclePrefab, WorldSpaceUI worldSpaceUIPrefab) {
        this.position = position;
        this.rotation = rotation;
        this.config = config;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
        this.collisionObstaclePrefab = collisionObstaclePrefab;
        this.avoidanceObstaclePrefab = avoidanceObstaclePrefab;
        this.visualsPrefab = visualsPrefab;
        this.pathfindingObstaclePrefab = pathfindingObstaclePrefab;
        this.worldSpaceUIPrefab = worldSpaceUIPrefab;
    }
}
