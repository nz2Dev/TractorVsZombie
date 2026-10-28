using System;
using UnityEngine;

[Serializable]
public struct FactionSource {
    [SerializeField] private bool alie;

    public FactionPrototype GetFactionPrototype() {
        return new FactionPrototype { alie = alie };
    }
}
