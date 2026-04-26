using UnityEngine;

public class PlatformSource : MonoBehaviour {

    [Inline] [SerializeField] private PlatformConfig config;
    [Local] [SerializeField] private PlatformVisuals visualsPrefab;
    [Local] [SerializeField] private VehiclePhysics physicsPrefab;
    [SerializeField] private Vector3 loadoutOffset;
    [Local] [SerializeField] private RamEffectSource ramSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public PlatformPrototype GetPrototype() {
        return new PlatformPrototype {
            position = transform.position,
            config = config,
            visualsPrefab = visualsPrefab,
            physicsPrefab = physicsPrefab,
            ramPrototype = ramSource != null ? ramSource.GetPrototype() : default,
            loadoutOffset = loadoutOffset,
        };
    }
}
