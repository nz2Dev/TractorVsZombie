using System;

using UnityEngine;

[Serializable]
public struct AimingSource {
    
    [SerializeField] private AimVisuals aimVisualsPrefab;
    
    public AimingPrototype Get() {
        return new AimingPrototype {
            aimVisualsPrefab = aimVisualsPrefab
        };
    }
}