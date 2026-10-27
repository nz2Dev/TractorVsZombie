using UnityEngine;

public class HeadquarterBuildingSource : MonoBehaviour {

    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [SerializeField] private HitboxSource hitboxSource;
    [SerializeField] private StructureSource structureSource;
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
            healthPrototype: healthSource.GetHealthPrototype(),
            factionPrototype: factionSource.GetFactionPrototype(),
            hitboxPrototype: hitboxSource.GetHitboxPrototype(),
            structurePrototype: structureSource.GetPrototype(transform.position),
            visualsPrefab: visualsPrefab,
            pathfindingObstaclePrefab: pathfindingObstaclePrefab,
            worldSpaceUIPrefab: worldSpaceUIPrefab
        );
    }

}
