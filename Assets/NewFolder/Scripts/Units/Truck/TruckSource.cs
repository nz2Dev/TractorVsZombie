using Compatibility;

using UnityEngine;

public class TruckSource : MonoBehaviour {

    [Inline] [SerializeField] private TruckConfig config;
    [Inline, SerializeField] private CombatAgentSource combatAgentSource;
    [Local] [SerializeField] private UnityVehicle vehiclePrefab;
    [Local] [SerializeField] private TruckVisuals visualsPrefab;
    [Local] [SerializeField] private RamEffectSource ramSource;
    [SerializeField] private AudioClip engineLoopSFX;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public TruckPrototype GetPrototype() {
        return new TruckPrototype {
            position = transform.position,
            rotation = transform.rotation,
            config = config,
            ramPrototype = ramSource.GetPrototype(),
            vehiclePrefab = vehiclePrefab,
            visualsPrefab = visualsPrefab,
            engineLoopSFX = engineLoopSFX,
            combatAgentPrototype = combatAgentSource.Get()
        };
    }
}
