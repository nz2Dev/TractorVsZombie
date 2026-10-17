using System;

using UnityEngine;

[Serializable]
public struct CollectingSource {
    
    [Inline, SerializeField] private CollectingConfig config;

    public CollectingPrototype Get() {
        return new CollectingPrototype {
            config = config
        };
    }
}