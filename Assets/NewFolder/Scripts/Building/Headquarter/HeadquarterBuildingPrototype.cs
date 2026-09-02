using Combat;

using UnityEngine;

public struct HeadquarterBuildingPrototype {
    
    public Vector3 position;
    public Quaternion rotation;
    public HeadquarterBuildingConfig config;
    public CombatPrototype combatPrototype;
    public RaycastMarker raycastMarkerPrefab;
    public CollisionObstacle collisionObstaclePrefab;
    public PhysicsObstacle avoidanceObstaclePrefab;
    public GameObject visualsPrefab;

    public HeadquarterBuildingPrototype(Vector3 position, Quaternion rotation, HeadquarterBuildingConfig config, 
        CombatPrototype combatPrototype, RaycastMarker raycastMarkerPrefab, CollisionObstacle collisionObstaclePrefab, 
        PhysicsObstacle avoidanceObstaclePrefab, GameObject visualsPrefab) {
        this.position = position;
        this.rotation = rotation;
        this.config = config;
        this.combatPrototype = combatPrototype;
        this.raycastMarkerPrefab = raycastMarkerPrefab;
        this.collisionObstaclePrefab = collisionObstaclePrefab;
        this.avoidanceObstaclePrefab = avoidanceObstaclePrefab;
        this.visualsPrefab = visualsPrefab;
    }
}
