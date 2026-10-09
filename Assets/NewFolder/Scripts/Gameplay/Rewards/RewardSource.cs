using UnityEngine;

public class RewardSource : MonoBehaviour {
    
    [Local] [SerializeField] private GameObject visualsPrefab;
    [SerializeField] private RewardType type;
    [SerializeField] private LoadoutSource loadoutSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public RewardPrototype GetPrototype() {
        return new RewardPrototype {
            position = transform.position,
            visualsPrefab = visualsPrefab,
            payload = new RewardPayload {
                type = type,
                loadoutPrototype = loadoutSource == null ? default : loadoutSource.GetPrototype(),
            }
        };
    }

}