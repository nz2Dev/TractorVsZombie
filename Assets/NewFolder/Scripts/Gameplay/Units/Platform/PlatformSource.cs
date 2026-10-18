using UnityEngine;

public class PlatformSource : MonoBehaviour {

    [Inline] [SerializeField] private PlatformConfig config;
    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [Local] [SerializeField] private PlatformVisuals visualsPrefab;
    [Local] [SerializeField] private UnityVehicle vehiclePrefab;
    [SerializeField] private Vector3 loadoutOffset; // TODO: consider making it visuals, by adding util TransformSource, that returns struct that contain Position/Rotation
    [Local] [SerializeField] private RamEffectSource ramSource;
    [Local, SerializeField] private RaycastMarker raycastMarkerPrefab;
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
            ramPrototype: ramSource != null ? ramSource.GetPrototype() : default,
            loadoutOffset: loadoutOffset,
            healthPrototype: healthSource.GetHealthPrototype(),
            factionPrototype: factionSource.GetFactionPrototype(),
            raycastMarkerPrefab: raycastMarkerPrefab,
            worldSpaceUIPrefab: worldSpaceUIPrefab
        );
    }
}
