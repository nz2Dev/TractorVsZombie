using System.ComponentModel;

using UnityEngine;

public class WeaponSource : MonoBehaviour {

    [Inline] [SerializeField] private WeaponConfig config;
    [Local] [SerializeField] private WeaponVisuals visualsPrefab;
    [SerializeField] private BallisticPrototypeSource ballisticPrototypeSource;
    [SerializeField] private Transform ballisticLaunchOffsetSource;
    [Space]
    [NonNull, SerializeField] private WeaponCategory category;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public WeaponPrototype GetPrototype() {
        return new WeaponPrototype {
            config = config,
            position = transform.position,
            visualsPrefab = visualsPrefab,
            ballisticPrototype = ballisticPrototypeSource.Get(),
            category = category,
        };
    }
}
