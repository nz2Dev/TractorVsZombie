using System;

using UnityEngine;

[Serializable]
public struct PlatformPresentationSource {
    [Local] public PlatformVisuals visualsPrefab;
    [NonNull, Local] public WorldSpaceUI worldSpaceUIPrefab;

    public PlatformPresentationPrototype Get() {
        return new PlatformPresentationPrototype {
            visualsPrefab = visualsPrefab,
            worldSpaceUIPrefab = worldSpaceUIPrefab
        };
    }
}
