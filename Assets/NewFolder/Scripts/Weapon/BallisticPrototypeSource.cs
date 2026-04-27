using System;

[Serializable]
public struct BallisticPrototypeSource {
    public BallisticType type;
    [Inline] public ProjectileConfig projectileConfig;
    public RocketPrototypeSource rocketPrototypeSource;

    public readonly BallisticPrototype Get() {
        return new BallisticPrototype {
            type = type,
            projectileConfig = projectileConfig,
            rocketPrototype = rocketPrototypeSource == null ? default : rocketPrototypeSource.Get(),
        };
    }
}