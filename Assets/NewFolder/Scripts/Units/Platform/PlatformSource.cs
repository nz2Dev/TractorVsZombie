using Compatibility;

using UnityEngine;

public class PlatformSource : MonoBehaviour {

    [Inline] [SerializeField] private PlatformConfig config;
    [Inline, SerializeField] private CombatPrototypeSource combatSource;
    [Local] [SerializeField] private PlatformVisuals visualsPrefab;
    [Local] [SerializeField] private UnityVehicle vehiclePrefab;
    [SerializeField] private Vector3 loadoutOffset; // TODO: consider making it visuals, by adding util TransformSource, that returns struct that contain Position/Rotation
    [Local] [SerializeField] private RamEffectSource ramSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public PlatformPrototype GetPrototype() {
        return new PlatformPrototype {
            position = transform.position,
            config = config,
            visualsPrefab = visualsPrefab,
            vehiclePrefab = vehiclePrefab,
            ramPrototype = ramSource != null ? ramSource.GetPrototype() : default,
            loadoutOffset = loadoutOffset,
            combatPrototype = combatSource.Get()
        };
    }
}
