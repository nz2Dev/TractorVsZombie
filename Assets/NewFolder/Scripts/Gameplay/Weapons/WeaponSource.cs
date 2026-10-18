using UnityEngine;

public class WeaponSource : MonoBehaviour {

    [Inline] [SerializeField] private WeaponConfig config;
    [Local] [SerializeField] private WeaponVisuals visualsPrefab;
    [SerializeField] private BallisticPrototypeSource ballisticPrototypeSource;
    [SerializeField] private Transform ballisticLaunchOffsetSource;


    private void Awake() {
        gameObject.SetActive(false);
    }

    public WeaponPrototype GetPrototype() {
        return new WeaponPrototype {
            config = config,
            position = transform.position,
            visualsPrefab = visualsPrefab,
            ballisticPrototype = ballisticPrototypeSource.Get(),
        };
    }
}
