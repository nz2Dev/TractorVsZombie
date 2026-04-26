using UnityEngine;

public class RamSource : MonoBehaviour {

    [Inline] [SerializeField] private RamConfig config;

    public RamPrototype GetPrototype() {
        return new RamPrototype {
            position = transform.position,
            config = config,
        };
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, config.radius);
    }
}
