using System;

using UnityEngine;

[Serializable]
public struct PlatformPresentationSource {
    [NonNull, Local] public PlatformVisuals visualsPrefab;
    [NonNull, Local] public PlatformStatsDisplay displayPrefab;

    public PlatformPresentationPrototype Get() {
        return new PlatformPresentationPrototype {
            visualsPrefab = visualsPrefab,
            displayPrefab = displayPrefab,
        };
    }
}
