using UnityEngine;

public class LoadoutSource : MonoBehaviour {

    [Inline] [SerializeField] private LoadoutConfig config;
    [Local] [SerializeField] private GameObject shellVisualsPrefab;
    [Local] [SerializeField] private WeaponSource weaponSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public LoadoutPrototype GetPrototype() {
        return new LoadoutPrototype {
            config = config,
            shellVisualsPrefab = shellVisualsPrefab,
            localWeaponPrototype = weaponSource != null ? weaponSource.GetPrototype(localTransform: true) : default,
        };
    }
}
