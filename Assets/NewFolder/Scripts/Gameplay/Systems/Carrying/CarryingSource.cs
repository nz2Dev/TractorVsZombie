using System;
using UnityEngine;

[Serializable]
public struct CarryingSource {
    [SerializeField] private Vector3 loadoutOffset;

    public CarryingPrototype Get() {
        return new CarryingPrototype { loadoutOffset = loadoutOffset };
    }
}
