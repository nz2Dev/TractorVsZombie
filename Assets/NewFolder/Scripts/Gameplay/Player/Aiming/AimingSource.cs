using UnityEngine;

public class AimingSource : MonoBehaviour {
    [SerializeField] private AimVisuals aimVisualsPrefab;
    
    public AimingPrototype Get() {
        return new AimingPrototype {
            aimVisualsPrefab = aimVisualsPrefab
        };
    }
}