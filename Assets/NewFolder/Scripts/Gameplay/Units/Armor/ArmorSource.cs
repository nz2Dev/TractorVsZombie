
using UnityEngine;

public class ArmorSource : MonoBehaviour {

    [SerializeField] private VehicleSource vehicleSource;
    [SerializeField] private RamSource ramSource;
    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [SerializeField] private AttachmentSource attachmentSource;
    [SerializeField] private HitboxSource hitboxSource;
    [SerializeField] private DropSource dropSource;
    [Space]
    [SerializeField] private ArmorPresentationSource presentationSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public ArmorPrototype GetPrototype() {
        return new ArmorPrototype(
            position: transform.position,
            vehiclePrototype: vehicleSource.GetPrototype(),
            attachmentPrototype: attachmentSource.Get(),
            ramPrototype: ramSource.GetPrototype(),
            dropPrototype: dropSource.Get(),
            healthPrototype: healthSource.GetHealthPrototype(),
            factionPrototype: factionSource.GetFactionPrototype(),
            hitboxPrototype: hitboxSource.GetHitboxPrototype(),
            presentationPrototype: presentationSource.Get()
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if (ramSource.config != null)
            Gizmos.DrawWireSphere(transform.position, ramSource.config.triggerRadius);
    }
#endif

}
