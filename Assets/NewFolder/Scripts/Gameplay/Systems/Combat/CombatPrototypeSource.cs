using Combat;

using UnityEngine;

public class CombatPrototypeSource : MonoBehaviour {
    
    [SerializeField] private bool alie;
    [Inline, SerializeField] private CombatConfig configSource;

    public CombatPrototype Get() {
        return new CombatPrototype {
            alie = alie,
            config = configSource,
        };
    }

}