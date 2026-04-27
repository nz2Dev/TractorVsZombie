using UnityEngine;

public class RewardSource : MonoBehaviour {
    
    [SerializeField] private RewardType type;
    [Local] [SerializeField] private GameObject visualsPrefab;
    [SerializeField] private LoadoutSource loadoutSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public RewardPrototype GetPrototype() {
        return new RewardPrototype {
            position = transform.position,
            type = type,
            visualsPrefab = visualsPrefab,
            loadoutPrototype = loadoutSource == null ? default : loadoutSource.GetPrototype(),
        };
    }

}