using System;
using UnityEngine;

[Serializable]
public struct CarryingSource {
    [NonNull, Local] public Transform loadoutOffset;
    [NonNull, Inline] public CarryingConfig config;

    public CarryingPrototype Get() {
        return new CarryingPrototype {
            loadoutOffset = loadoutOffset.localPosition,
            config = config,
        };
    }
}
