using System;

[Serializable]
public struct BallisticPrototypeSource {
    public BallisticType type;
    [Inline] public ProjectileSource projectileSource;
    [Inline] public RocketPrototypeSource rocketPrototypeSource;

    public readonly BallisticPrototype Get() {
        return new BallisticPrototype {
            type = type,
            projectilePrototype = projectileSource == null ? default : projectileSource.Get(),
            rocketPrototype = rocketPrototypeSource == null ? default : rocketPrototypeSource.Get(),
        };
    }
}