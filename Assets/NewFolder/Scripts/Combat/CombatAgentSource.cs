using UnityEngine;

public class CombatAgentSource : MonoBehaviour {

    [SerializeField] private bool alie = false;
    [SerializeField] private CombatAgentConfig configSource;
    [SerializeField] private CombatAgentCollider colliderSource;

    public CombatAgentPrototype Get() {
        return new CombatAgentPrototype {
            alie = alie,
            config = configSource,
            collider = colliderSource,
        };
    }
}