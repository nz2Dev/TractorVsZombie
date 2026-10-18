
using UnityEngine;

[System.Serializable]
public struct ArmorPrototype {
    
    public Vector3 position;
    public UnityVehicle vehiclePrefab;
    public AttachmentPrototype attachmentPrototype;
    public RamEffectPrototype ramPrototype;
    public DropPrototype dropPrototype;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public ArmorAudioPrototype armorAudioPrototype;
    public ArmorUIPrototype armorUIPrototype;
    public ArmorViewPrototype armorViewPrototype;

    public ArmorPrototype(Vector3 position, UnityVehicle vehiclePrefab, AttachmentPrototype attachmentPrototype, RamEffectPrototype ramPrototype, DropPrototype dropPrototype, HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype, ArmorAudioPrototype armorAudioPrototype, ArmorUIPrototype armorUIPrototype, ArmorViewPrototype armorViewPrototype) {
        this.position = position;
        this.vehiclePrefab = vehiclePrefab;
        this.attachmentPrototype = attachmentPrototype;
        this.ramPrototype = ramPrototype;
        this.dropPrototype = dropPrototype;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
        this.armorAudioPrototype = armorAudioPrototype;
        this.armorUIPrototype = armorUIPrototype;
        this.armorViewPrototype = armorViewPrototype;
    }
}
