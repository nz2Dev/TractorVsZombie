using UnityEngine;
using UnityEngine.Serialization;

public class HeadquarterBuildingSource : MonoBehaviour {

    [Inline, SerializeField] private HeadquarterBuildingConfig config;
    [FormerlySerializedAs("combatSource"), SerializeField] private HealthFactionSource healthFactionSource;
    [NonNull, Local, SerializeField] private RaycastMarker raycastMarkerPrefab;
    [NonNull, Local, SerializeField] private CollisionObstacle collisionObstaclePrefab;
    [NonNull, Local, SerializeField] private ORCAObstacleVertices avoidanceObstaclePrefab;
    [NonNull, Local, SerializeField] private Collider pathfindingObstaclePrefab;
    [NonNull, Local, SerializeField] private HeadquarterBuildingVisuals visualsPrefab;
    [NonNull, Local, SerializeField] private WorldSpaceUI worldSpaceUIPrefab;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public HeadquarterBuildingPrototype GetPrototype() {
        return new HeadquarterBuildingPrototype (
            position: transform.position,
            rotation: transform.rotation,
            config: config,
            healthPrototype: healthFactionSource.GetHealthPrototype(),
            factionPrototype: healthFactionSource.GetFactionPrototype(),
            raycastMarkerPrefab: raycastMarkerPrefab,
            collisionObstaclePrefab: collisionObstaclePrefab,
            avoidanceObstaclePrefab: avoidanceObstaclePrefab,
            visualsPrefab: visualsPrefab,
            pathfindingObstaclePrefab: pathfindingObstaclePrefab,
            worldSpaceUIPrefab: worldSpaceUIPrefab
        );
    }

}
