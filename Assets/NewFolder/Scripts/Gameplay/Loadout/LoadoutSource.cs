using UnityEngine;

public class LoadoutSource : MonoBehaviour {

    [SerializeField] private HealthSource healthSource;
    [SerializeField] private HitboxSource hitboxSource;
    [Local, NonNull, SerializeField] private WeaponSource weaponSource;
    [Space]
    [Local] [SerializeField] private LoadoutVisuals visualsPrefab;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public LoadoutPrototype GetPrototype() {
        return new LoadoutPrototype {
            visualsPrefab = visualsPrefab,
            healthPrototype = healthSource.GetHealthPrototype(),
            hitboxPrototype = hitboxSource.GetHitboxPrototype(),
            weaponPrototype = weaponSource.GetPrototype(),
            weaponOffset = weaponSource.transform.localPosition,
        };
    }
}
