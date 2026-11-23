using System;

using UnityEngine;

[Serializable]
public struct TruckPresentationSource {
    [Local] public TruckVisuals visualsPrefab;
    public AudioClip engineLoopSFX;
    [Local] public AudioSource ramImpactAudioSourcePrefab;
    public AudioClip[] ramImpactSFX;
    [NonNull, Local] public TruckStatsDisplay statsDisplayPrefab;
    [NonNull, Local] public TruckOutlineDisplay outlineDisplayPrefab;

    public TruckPresentationPrototype Get() {
        return new TruckPresentationPrototype {
            visualsPrefab = visualsPrefab,
            engineLoopSFX = engineLoopSFX,
            ramImpactAudioSourcePrefab = ramImpactAudioSourcePrefab,
            ramImpactSFX = ramImpactSFX,
            statsDisplayPrefab = statsDisplayPrefab,
            outlineDisplayPrefab = outlineDisplayPrefab,
        };
    }
}
