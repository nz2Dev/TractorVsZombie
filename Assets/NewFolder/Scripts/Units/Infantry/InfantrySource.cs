using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InfantrySource : MonoBehaviour {

    [Inline] [SerializeField] private InfantryConfig config;
    [Local] [SerializeField] private InfantryVisuals visualsPrefab;
    [SerializeField] private RewardSource rewardSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public InfantryPrototype GetPrototype() {
        return new InfantryPrototype {
            position = transform.position,
            config = config,
            visualsPrefab = visualsPrefab,
            rewardPrototype = rewardSource.GetPrototype(),
        };
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (config == null)
            return;

        Handles.DrawWireDisc(transform.position, Vector3.up, config.bodyData.radius);
        Handles.DrawWireDisc(transform.position + Vector3.up * config.bodyData.height, Vector3.up, config.bodyData.radius);

        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position + Vector3.up * config.hitboxHeight * 0.5f, Vector3.up, config.hitboxRadius);
    }
#endif
}
