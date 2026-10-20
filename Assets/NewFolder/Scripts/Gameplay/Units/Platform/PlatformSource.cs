using UnityEngine;

public class PlatformSource : MonoBehaviour {

    [Inline] [SerializeField] private PlatformConfig config;
    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [Local] [SerializeField] private PlatformVisuals visualsPrefab;
    [Local] [SerializeField] private UnityVehicle vehiclePrefab;
    [SerializeField] private Vector3 loadoutOffset; // TODO: consider making it visuals, by adding util TransformSource, that returns struct that contain Position/Rotation
    [SerializeField] private HitboxSource hitboxSource;
    [NonNull, Local, SerializeField] private WorldSpaceUI worldSpaceUIPrefab;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public PlatformPrototype GetPrototype() {
        return new PlatformPrototype (
            position: transform.position,
            config: config,
            visualsPrefab: visualsPrefab,
            vehiclePrefab: vehiclePrefab,
            loadoutOffset: loadoutOffset,
            healthPrototype: healthSource.GetHealthPrototype(),
            factionPrototype: factionSource.GetFactionPrototype(),
            hitboxPrototype: hitboxSource.GetHitboxPrototype(),
            worldSpaceUIPrefab: worldSpaceUIPrefab
        );
    }
}
