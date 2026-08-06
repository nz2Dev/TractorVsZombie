using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InfantrySource : MonoBehaviour {

    [Inline] [SerializeField] private InfantryConfig config;
    [Local] [SerializeField] private InfantryVisuals visualsPrefab;
    // setting the layer to the one that vehicle physics can interact with give interesting results
    // when the body is in "dynamic" state
    [Local] [SerializeField] private PhysicsBody physicsBodyPrefab; 
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
            physicsBodyPrefab = physicsBodyPrefab,
        };
    }

}
