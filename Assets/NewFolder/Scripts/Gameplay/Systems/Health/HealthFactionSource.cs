using System;
using UnityEngine;

[Serializable]
public struct HealthFactionSource {
    [SerializeField] private bool alie;
    [Inline, SerializeField] private HealthConfig configSO;

    public HealthPrototype GetHealthPrototype() {
        return new HealthPrototype {
            config = configSO,
        };
    }

    public FactionPrototype GetFactionPrototype() {
        return new FactionPrototype { alie = alie };
    }
}
