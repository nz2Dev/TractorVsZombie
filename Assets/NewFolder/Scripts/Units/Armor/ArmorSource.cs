using UnityEngine;

public class ArmorSource : MonoBehaviour {

    [Inline] [SerializeField] private ArmorConfig config;
    [Local] [SerializeField] private ArmorVisuals visualsPrefab;
    [Local] [SerializeField] private VehiclePhysics physicsPrefab;
    [SerializeField] private AudioClip engineLoopSFX;
    [SerializeField] private Vector3 weaponPlacementOffset;
    [Local] [SerializeField] private WeaponSource weaponSource;
    [Local] [SerializeField] private RamEffectSource ramSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public ArmorPrototype GetPrototype() {
        return new ArmorPrototype {
            position = transform.position,
            config = config,
            visualsPrefab = visualsPrefab,
            physicsPrefab = physicsPrefab,
            engineLoopSFX = engineLoopSFX,
            weaponPlacementOffset = weaponPlacementOffset,
            weaponPrototype = weaponSource != null ? weaponSource.GetPrototype() : default,
            ramPrototype = ramSource != null ? ramSource.GetPrototype() : default,
        };
    }
}
