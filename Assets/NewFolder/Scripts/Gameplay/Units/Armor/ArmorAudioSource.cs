using System;

using UnityEngine;

[Serializable]
public struct ArmorAudioSource {
    public AudioClip engineLoopSFX;

    public ArmorAudioPrototype Get() {
        return new ArmorAudioPrototype {
            engineLoopSFX = engineLoopSFX,
        };
    }
}