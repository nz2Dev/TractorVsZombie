using System;

using UnityEngine;

[Serializable]
public struct SpawnVariantSource {
    
    public SpawnType type;
    public InfantrySource infantrySource;
    public ArmorSource armorSource;

    public readonly SpawnVariant Get() {
        return new SpawnVariant {
            type = type,
            infantryPrototype = infantrySource == null ? default : infantrySource.GetPrototype(),
            armorPrototype = armorSource == null ? default : armorSource.GetPrototype()
        };
    }
}