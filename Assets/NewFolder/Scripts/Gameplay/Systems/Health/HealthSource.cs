using System;
using UnityEngine;

[Serializable]
public struct HealthSource {
    [Inline, SerializeField] private HealthConfig configSO;

    public HealthPrototype GetHealthPrototype() {
        return new HealthPrototype {
            config = configSO,
        };
    }
}
