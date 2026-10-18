using UnityEngine;

public class LoadoutSource : MonoBehaviour {

    [Inline] [SerializeField] private LoadoutConfig config;
    [Local] [SerializeField] private GameObject shellVisualsPrefab;
    [Local, NonNull, SerializeField] private WeaponSource weaponSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public LoadoutPrototype GetPrototype() {
        return new LoadoutPrototype {
            config = config,
            shellVisualsPrefab = shellVisualsPrefab,
            weaponPrototype = weaponSource.GetPrototype(),
            weaponOffset = weaponSource.transform.localPosition,
        };
    }
}
