using UnityEngine;

public class RocketPrototypeSource : MonoBehaviour {

    [Inline] [SerializeField] private RocketConfig config;
    [Local] [SerializeField] private RocketVisuals visualsPrefab;

    public RocketPrototype Get() {
        return new RocketPrototype {
            config = config,
            visualsPrefab = visualsPrefab
        };
    }
}
