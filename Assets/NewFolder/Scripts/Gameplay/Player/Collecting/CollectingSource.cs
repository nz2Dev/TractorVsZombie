using UnityEngine;

public class CollectingSource : MonoBehaviour {
    
    [Inline, SerializeField] private CollectingConfig config;

    public CollectingPrototype Get() {
        return new CollectingPrototype {
            config = config
        };
    }
}