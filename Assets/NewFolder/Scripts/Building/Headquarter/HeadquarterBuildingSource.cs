using UnityEngine;

public class HeadquarterBuildingSource : MonoBehaviour {

    [Inline, SerializeField] private HeadquarterBuildingConfig config;
    [Inline, SerializeField] private CombatPrototypeSource combatSource;
    [NonNull, Local, SerializeField] private RaycastMarker raycastMarkerPrefab;
    [NonNull, Local, SerializeField] private PhysicsObstacle vehicleObstaclePrefab;
    [NonNull, Local, SerializeField] private RagdollObstacle physicsObstaclePrefab;
    [NonNull, Local, SerializeField] private PhysicsObstacle avoidanceObstaclePrefab;
    [NonNull, Local, SerializeField] private GameObject visualsPrefab;

    public HeadquarterBuildingPrototype GetPrototype() {
        return new HeadquarterBuildingPrototype (
            position: transform.position,
            rotation: transform.rotation,
            config: config,
            combatPrototype: combatSource.Get(),
            raycastMarkerPrefab: raycastMarkerPrefab,
            vehicleObstaclePrefab: vehicleObstaclePrefab,
            physicsObstaclePrefab: physicsObstaclePrefab,
            avoidanceObstaclePrefab: avoidanceObstaclePrefab,
            visualsPrefab: visualsPrefab
        );
    }
}
