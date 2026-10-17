using System;

using Combat;

using UnityEngine;

[Serializable]
public struct CombatSource {
    [SerializeField] private bool alie;
    [Inline, SerializeField] private CombatConfig configSO;

    public CombatPrototype Get() {
        return new CombatPrototype {
            alie = alie,
            config = configSO,
        };
    }
}