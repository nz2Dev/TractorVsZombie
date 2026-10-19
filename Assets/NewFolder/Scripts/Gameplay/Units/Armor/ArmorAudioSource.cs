using System;

using UnityEngine;

[Serializable]
public struct ArmorAudioSource {
    
    [Inline] public ArmorAudioConfig configSO;

    public ArmorAudioPrototype Get() {
        return new ArmorAudioPrototype {
            config = configSO,
        };
    }
}