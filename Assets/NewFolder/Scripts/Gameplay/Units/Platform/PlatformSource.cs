using UnityEngine;

public class PlatformSource : MonoBehaviour {

    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [Local] [SerializeField] private PlatformVisuals visualsPrefab;
    [SerializeField] private VehicleSource vehicleSource;
    [SerializeField] private Vector3 loadoutOffset; // TODO: consider making it visuals, by adding util TransformSource, that returns struct that contain Position/Rotation
    [SerializeField] private HitboxSource hitboxSource;
    [NonNull, Local, SerializeField] private WorldSpaceUI worldSpaceUIPrefab;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public PlatformPrototype GetPrototype() {
        return new PlatformPrototype (
            position: transform.position,
            visualsPrefab: visualsPrefab,
            vehiclePrototype: vehicleSource.GetPrototype(),
            loadoutOffset: loadoutOffset,
            healthPrototype: healthSource.GetHealthPrototype(),
            factionPrototype: factionSource.GetFactionPrototype(),
            hitboxPrototype: hitboxSource.GetHitboxPrototype(),
            worldSpaceUIPrefab: worldSpaceUIPrefab
        );
    }
}
