using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InfantrySource : MonoBehaviour {

    [Inline] [SerializeField] private InfantryConfig config;
    [Local] [SerializeField] private InfantryVisuals visualsPrefab;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public InfantryPrototype GetPrototype() {
        return new InfantryPrototype {
            position = transform.position,
            config = config,
            visualsPrefab = visualsPrefab,
        };
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (config == null)
            return;

        Handles.DrawWireDisc(transform.position, Vector3.up, config.bodyData.radius);
        Handles.DrawWireDisc(transform.position + Vector3.up * config.bodyData.height, Vector3.up, config.bodyData.radius);
    }
#endif
}
