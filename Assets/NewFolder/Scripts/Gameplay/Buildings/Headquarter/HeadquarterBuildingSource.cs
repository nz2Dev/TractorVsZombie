using UnityEngine;

public class HeadquarterBuildingSource : MonoBehaviour {

    [Inline, SerializeField] private HeadquarterBuildingConfig config;
    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [SerializeField] private HitboxSource hitboxSource;
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
            healthPrototype: healthSource.GetHealthPrototype(),
            factionPrototype: factionSource.GetFactionPrototype(),
            hitboxPrototype: hitboxSource.GetHitboxPrototype(),
            collisionObstaclePrefab: collisionObstaclePrefab,
            avoidanceObstaclePrefab: avoidanceObstaclePrefab,
            visualsPrefab: visualsPrefab,
            pathfindingObstaclePrefab: pathfindingObstaclePrefab,
            worldSpaceUIPrefab: worldSpaceUIPrefab
        );
    }

}
