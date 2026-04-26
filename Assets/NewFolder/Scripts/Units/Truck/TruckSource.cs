using UnityEngine;

public class TruckSource : MonoBehaviour {

    [Inline] [SerializeField] private TruckConfig config;
    [Local] [SerializeField] private VehiclePhysics vehiclePhysicsPrefab;
    [Local] [SerializeField] private TruckVisuals visualsPrefab;
    [SerializeField] private AudioClip engineLoopSFX;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public TruckPrototype GetPrototype() {
        return new TruckPrototype {
            position = transform.position,
            config = config,
            vehiclePhysicsPrefab = vehiclePhysicsPrefab,
            visualsPrefab = visualsPrefab,
            engineLoopSFX = engineLoopSFX,
        };
    }
}
