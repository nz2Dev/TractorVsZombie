using System;

using Compatibility;

using UnityEngine;

public class HeadquarterBuildingSource : MonoBehaviour {

    [Inline, SerializeField] private HeadquarterBuildingConfig config;
    [Inline, SerializeField] private CombatAgentSource combatAgentSource;

    public HeadquarterBuildingPrototype GetPrototype() {
        return new HeadquarterBuildingPrototype {
            position = transform.position,
            rotation = transform.rotation,
            config = config,
            combatAgentPrototype = combatAgentSource.Get(),
        };
    }
}
