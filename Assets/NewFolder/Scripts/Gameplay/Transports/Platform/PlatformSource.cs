using UnityEngine;

public class PlatformSource : MonoBehaviour {

    [SerializeField] private FactionSource factionSource;
    [SerializeField] private VehicleSource vehicleSource;
    [SerializeField] private CarryingSource carryingSource;
    [Space]
    [SerializeField] private PlatformPresentationSource presentationSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public PlatformPrototype GetPrototype() {
        return new PlatformPrototype (
            position: transform.position,
            presentationPrototype: presentationSource.Get(),
            vehiclePrototype: vehicleSource.GetPrototype(),
            carryingPrototype: carryingSource.Get(),
            factionPrototype: factionSource.GetFactionPrototype()
        );
    }
}
