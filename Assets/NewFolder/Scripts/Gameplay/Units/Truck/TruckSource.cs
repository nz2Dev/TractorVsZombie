using UnityEngine;
using UnityEngine.Serialization;

public class TruckSource : MonoBehaviour {

    [Inline] [SerializeField] private TruckConfig config;
    [FormerlySerializedAs("combatSource"), SerializeField] private HealthFactionSource healthFactionSource;
    [Local] [SerializeField] private UnityVehicle vehiclePrefab;
    [Local] [SerializeField] private TruckVisuals visualsPrefab;
    [Local] [SerializeField] private RamEffectSource ramSource;
    [SerializeField] private AudioClip engineLoopSFX;
    [NonNull, Local, SerializeField] private RaycastMarker raycastMarkerPrefab;
    [NonNull, Local, SerializeField] private WorldSpaceUI worldSpaceUIPrefab;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public TruckPrototype GetPrototype() {
        return new TruckPrototype {
            position = transform.position,
            rotation = transform.rotation,
            config = config,
            ramPrototype = ramSource.GetPrototype(),
            vehiclePrefab = vehiclePrefab,
            visualsPrefab = visualsPrefab,
            engineLoopSFX = engineLoopSFX,
            healthPrototype = healthFactionSource.GetHealthPrototype(),
            factionPrototype = healthFactionSource.GetFactionPrototype(),
            raycastMarkerPrefab = raycastMarkerPrefab,
            worldSpaceUIPrefab = worldSpaceUIPrefab
        };
    }
}
