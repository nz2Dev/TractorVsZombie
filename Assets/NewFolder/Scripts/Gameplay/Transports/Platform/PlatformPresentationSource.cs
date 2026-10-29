using System;

using UnityEngine;

[Serializable]
public struct PlatformPresentationSource {
    [Local] public PlatformVisuals visualsPrefab;
    [NonNull, Local] public PlatformUIElements platformElementsPrefab;

    public PlatformPresentationPrototype Get() {
        return new PlatformPresentationPrototype {
            visualsPrefab = visualsPrefab,
            platformElementsPrefab = platformElementsPrefab,
        };
    }
}
