using UnityEngine;

public class CombatAgentSource : MonoBehaviour {

    [SerializeField] private bool alie = false;
    [SerializeField] private CombatAgentConfig configSource;
    [Local, SerializeField] private CollisionMarker markerPrefab;

    public CombatAgentPrototype Get() {
        return new CombatAgentPrototype {
            alie = alie,
            config = configSource,
            markerPrefab = markerPrefab,
        };
    }
}