using System;

using UnityEngine;

[Serializable]
public struct ArmorPresentationSource {

    public AudioClip engineLoopSFX;
    public ArmorVisuals visualsPrefab;
    [NonNull, Local] public WorldSpaceUI worldSpaceUIPrefab;

    public ArmorPresentationPrototype Get() {
        return new ArmorPresentationPrototype {
            engineLoopSFX = engineLoopSFX,
            visualsPrefab = visualsPrefab,
            worldSpaceUIPrefab = worldSpaceUIPrefab
        };
    }
}
