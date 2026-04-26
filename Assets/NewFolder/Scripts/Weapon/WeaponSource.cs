using UnityEngine;

public class WeaponSource : MonoBehaviour {

    [Inline] [SerializeField] private WeaponConfig config;
    [Local] [SerializeField] private WeaponVisuals visualsPrefab;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public WeaponPrototype GetPrototype(bool localTransform) {
        return new WeaponPrototype {
            position = localTransform ? transform.localPosition : transform.position,
            config = config,
            visualsPrefab = visualsPrefab,
        };
    }
}
