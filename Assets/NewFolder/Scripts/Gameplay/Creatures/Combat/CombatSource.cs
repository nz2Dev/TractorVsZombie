using System;

[Serializable]
public struct CombatSource {
    [Inline] public CombatConfig config;

    public CombatPrototype GetPrototype() => new() {
        config = config
    };
}
