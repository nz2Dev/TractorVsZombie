using System;

using UnityEngine;

[Serializable]
public struct PlatformPresentationSource {
    [NonNull, Local] public PlatformVisuals visualsPrefab;
    [NonNull, Local] public PlatformStatsDisplay statsDisplayPrefab;
    [NonNull, Local] public PlatformOutlineDisplay outlineDisplayPrefab;

    public PlatformPresentationPrototype Get() {
        return new PlatformPresentationPrototype {
            visualsPrefab = visualsPrefab,
            statsDisplayPrefab = statsDisplayPrefab,
            outlineDisplayPrefab = outlineDisplayPrefab
        };
    }
}
