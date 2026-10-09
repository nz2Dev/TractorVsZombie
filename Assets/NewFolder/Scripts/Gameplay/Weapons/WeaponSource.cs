using UnityEngine;

public class WeaponSource : MonoBehaviour {

    [Inline] [SerializeField] private WeaponConfig config;
    [Local] [SerializeField] private WeaponVisuals visualsPrefab;
    [SerializeField] private BallisticPrototypeSource ballisticPrototypeSource;
    [SerializeField] private Transform ballisticLaunchOffsetSource;


    private void Awake() {
        gameObject.SetActive(false);
    }

    public WeaponPrototype GetPrototype(bool localTransform) {
        return new WeaponPrototype {
            config = config,
            position = localTransform ? transform.localPosition : transform.position,
            visualsPrefab = visualsPrefab,
            ballisticPrototype = ballisticPrototypeSource.Get(),
        };
    }
}
