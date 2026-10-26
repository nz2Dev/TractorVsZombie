using UnityEngine;

public class PlatformSource : MonoBehaviour {

    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [Local] [SerializeField] private PlatformVisuals visualsPrefab;
    [SerializeField] private VehicleSource vehicleSource;
    [SerializeField] private CarryingSource carryingSource;
    [SerializeField] private HitboxSource hitboxSource;
    [NonNull, Local, SerializeField] private WorldSpaceUI worldSpaceUIPrefab;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public PlatformPrototype GetPrototype() {
        return new PlatformPrototype (
            position: transform.position,
            visualsPrefab: visualsPrefab,
            vehiclePrototype: vehicleSource.GetPrototype(),
            carryingPrototype: carryingSource.Get(),
            healthPrototype: healthSource.GetHealthPrototype(),
            factionPrototype: factionSource.GetFactionPrototype(),
            hitboxPrototype: hitboxSource.GetHitboxPrototype(),
            worldSpaceUIPrefab: worldSpaceUIPrefab
        );
    }
}
