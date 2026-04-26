using UnityEngine;

public class RamEffectSource : MonoBehaviour {

    [Inline] [SerializeField] private RamEffectConfig config;

    public RamEffectPrototype GetPrototype() {
        return new RamEffectPrototype {
            position = transform.position,
            config = config,
        };
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, config.radius);
    }
}
