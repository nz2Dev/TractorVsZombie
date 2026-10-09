using Combat;

using UnityEngine;

public struct HeadquarterBuildingPrototype {
    
    public Vector3 position;
    public Quaternion rotation;
    public HeadquarterBuildingConfig config;
    public CombatPrototype combatPrototype;
    public RaycastMarker raycastMarkerPrefab;
    public CollisionObstacle collisionObstaclePrefab;
    public ORCAObstacleVertices avoidanceObstaclePrefab;
    public Collider pathfindingObstaclePrefab;
    public HeadquarterBuildingVisuals visualsPrefab;
    public WorldSpaceUI worldSpaceUIPrefab;

    public HeadquarterBuildingPrototype(Vector3 position, Quaternion rotation, HeadquarterBuildingConfig config,
        CombatPrototype combatPrototype, RaycastMarker raycastMarkerPrefab, CollisionObstacle collisionObstaclePrefab,
        ORCAObstacleVertices avoidanceObstaclePrefab, HeadquarterBuildingVisuals visualsPrefab, Collider pathfindingObstaclePrefab, WorldSpaceUI worldSpaceUIPrefab) {
        this.position = position;
        this.rotation = rotation;
        this.config = config;
        this.combatPrototype = combatPrototype;
        this.raycastMarkerPrefab = raycastMarkerPrefab;
        this.collisionObstaclePrefab = collisionObstaclePrefab;
        this.avoidanceObstaclePrefab = avoidanceObstaclePrefab;
        this.visualsPrefab = visualsPrefab;
        this.pathfindingObstaclePrefab = pathfindingObstaclePrefab;
        this.worldSpaceUIPrefab = worldSpaceUIPrefab;
    }
}
