using System;
using UnityEngine;

[Serializable]
public struct CarryingSource {
    [NonNull, Local] public Transform loadoutOffset;

    public CarryingPrototype Get() {
        return new CarryingPrototype { loadoutOffset = loadoutOffset.localPosition };
    }
}
