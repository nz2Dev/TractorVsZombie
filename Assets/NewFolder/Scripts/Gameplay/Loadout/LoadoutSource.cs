using UnityEngine;

public class LoadoutSource : MonoBehaviour {

    [Local] [SerializeField] private GameObject shellVisualsPrefab;
    [Local, NonNull, SerializeField] private WeaponSource weaponSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public LoadoutPrototype GetPrototype() {
        return new LoadoutPrototype {
            shellVisualsPrefab = shellVisualsPrefab,
            weaponPrototype = weaponSource.GetPrototype(),
            weaponOffset = weaponSource.transform.localPosition,
        };
    }
}
