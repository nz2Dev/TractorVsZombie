using Combat;

using UnityEngine;

public struct HeadquarterBuildingPrototype {
    
    public Vector3 position;
    public Quaternion rotation;
    public HeadquarterBuildingConfig config;
    public CombatPrototype combatPrototype;
    public RaycastMarker raycastMarkerPrefab;
    public PhysicsObstacle vehicleObstaclePrefab;
    public RagdollObstacle physicsObstaclePrefab;
    public PhysicsObstacle avoidanceObstaclePrefab;
    public GameObject visualsPrefab;

    public HeadquarterBuildingPrototype(Vector3 position, Quaternion rotation, HeadquarterBuildingConfig config, CombatPrototype combatPrototype, RaycastMarker raycastMarkerPrefab, PhysicsObstacle vehicleObstaclePrefab, RagdollObstacle physicsObstaclePrefab, PhysicsObstacle avoidanceObstaclePrefab, GameObject visualsPrefab) {
        this.position = position;
        this.rotation = rotation;
        this.config = config;
        this.combatPrototype = combatPrototype;
        this.raycastMarkerPrefab = raycastMarkerPrefab;
        this.vehicleObstaclePrefab = vehicleObstaclePrefab;
        this.physicsObstaclePrefab = physicsObstaclePrefab;
        this.avoidanceObstaclePrefab = avoidanceObstaclePrefab;
        this.visualsPrefab = visualsPrefab;
    }
}
