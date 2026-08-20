using System;

using Compatibility;

using UnityEngine;

public class HeadquarterBuildingSource : MonoBehaviour {

    [Inline, SerializeField] private HeadquarterBuildingConfig config;
    [Inline, SerializeField] private CombatPrototypeSource combatSource;

    public HeadquarterBuildingPrototype GetPrototype() {
        return new HeadquarterBuildingPrototype {
            position = transform.position,
            rotation = transform.rotation,
            config = config,
            combatPrototype = combatSource.Get(),
        };
    }
}
