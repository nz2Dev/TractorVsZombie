using UnityEngine;

public class RamEffectSource : MonoBehaviour {

    [Inline] [SerializeField] private RamEffectConfig config;

    public RamEffectPrototype GetPrototype() {
        return new RamEffectPrototype {
            position = transform.position,
            config = config,
        };
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if (config != null) {
            Gizmos.DrawWireSphere(transform.position, config.radius);
        }
    }
}
#endif
