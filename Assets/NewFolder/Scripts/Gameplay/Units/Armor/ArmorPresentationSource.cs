using System;

using UnityEngine;

[Serializable]
public struct ArmorPresentationSource {

    public AudioClip engineLoopSFX;
    [Local] public AudioSource ramImpactAudioSourcePrefab;
    public AudioClip[] ramImpactSFX;
    public ArmorVisuals visualsPrefab;
    [NonNull, Local] public WorldSpaceUI worldSpaceUIPrefab;

    public ArmorPresentationPrototype Get() {
        return new ArmorPresentationPrototype {
            engineLoopSFX = engineLoopSFX,
            ramImpactAudioSourcePrefab = ramImpactAudioSourcePrefab,
            ramImpactSFX = ramImpactSFX,
            visualsPrefab = visualsPrefab,
            worldSpaceUIPrefab = worldSpaceUIPrefab
        };
    }
}
