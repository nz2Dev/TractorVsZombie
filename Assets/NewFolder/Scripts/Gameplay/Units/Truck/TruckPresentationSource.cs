using System;

using UnityEngine;

[Serializable]
public struct TruckPresentationSource {
    [Local] public TruckVisuals visualsPrefab;
    public AudioClip engineLoopSFX;
    [NonNull, Local] public WorldSpaceUI worldSpaceUIPrefab;

    public TruckPresentationPrototype Get() {
        return new TruckPresentationPrototype {
            visualsPrefab = visualsPrefab,
            engineLoopSFX = engineLoopSFX,
            worldSpaceUIPrefab = worldSpaceUIPrefab
        };
    }
}
