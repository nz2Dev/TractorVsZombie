
using UnityEngine;

public class ArmorSource : MonoBehaviour {

    [Local] [SerializeField] private UnityVehicle vehiclePrefab;
    [Local] [SerializeField] private RamEffectSource ramSource;
    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [SerializeField] private AttachmentSource attachmentSource;
    [SerializeField] private HitboxSource hitboxSource;
    [SerializeField] private DropSource dropSource;
    [Space]
    [SerializeField] private ArmorViewSource viewSource;
    [SerializeField] private ArmorAudioSource audioSource;
    [SerializeField] private ArmorUISource uiSource;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public ArmorPrototype GetPrototype() {
        return new ArmorPrototype (
            position: transform.position,
            vehiclePrefab: vehiclePrefab,
            attachmentPrototype: attachmentSource.Get(),
            ramPrototype: ramSource != null ? ramSource.GetPrototype() : default,
            dropPrototype: dropSource.Get(),
            healthPrototype: healthSource.GetHealthPrototype(),
            factionPrototype: factionSource.GetFactionPrototype(),
            hitboxPrototype: hitboxSource.GetHitboxPrototype(),
            armorAudioPrototype: audioSource.Get(),
            armorUIPrototype: uiSource.Get(),
            armorViewPrototype: viewSource.Get()
        );
    }
}
