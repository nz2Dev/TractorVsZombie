using UnityEngine;

public class TruckSource : MonoBehaviour {

    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [Local] [SerializeField] private UnityVehicle vehiclePrefab;
    [Local] [SerializeField] private RamEffectSource ramSource;
    [SerializeField] private HitboxSource hitboxSource;
    [Space]
    [SerializeField] private TruckPresentationSource presentationSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public TruckPrototype GetPrototype() {
        return new TruckPrototype {
            position = transform.position,
            rotation = transform.rotation,
            ramPrototype = ramSource.GetPrototype(),
            vehiclePrefab = vehiclePrefab,
            presentationPrototype = presentationSource.Get(),
            healthPrototype = healthSource.GetHealthPrototype(),
            factionPrototype = factionSource.GetFactionPrototype(),
            hitboxPrototype = hitboxSource.GetHitboxPrototype()
        };
    }
}
