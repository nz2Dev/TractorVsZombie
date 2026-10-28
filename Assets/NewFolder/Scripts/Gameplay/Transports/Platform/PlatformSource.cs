using UnityEngine;

public class PlatformSource : MonoBehaviour {

    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [SerializeField] private VehicleSource vehicleSource;
    [SerializeField] private CarryingSource carryingSource;
    [SerializeField] private HitboxSource hitboxSource;
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
            healthPrototype: healthSource.GetHealthPrototype(),
            factionPrototype: factionSource.GetFactionPrototype(),
            hitboxPrototype: hitboxSource.GetHitboxPrototype()
        );
    }
}
