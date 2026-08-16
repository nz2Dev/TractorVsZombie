using UnityEngine;

public class RamEffectSource : MonoBehaviour {

    [Inline] [SerializeField] private RamEffectConfig config;
    [Local] [SerializeField] private AudioSource audioSourcePrefab;

    public RamEffectPrototype GetPrototype() {
        return new RamEffectPrototype {
            position = transform.position,
            config = config,
            audioSourcePrefab = audioSourcePrefab
        };
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if (config != null) {
            Gizmos.DrawWireSphere(transform.position, config.triggerRadius);
        }
    }
}
#endif
