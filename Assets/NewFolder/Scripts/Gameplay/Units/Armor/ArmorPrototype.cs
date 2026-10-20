
using UnityEngine;

[System.Serializable]
public struct ArmorPrototype {
    
    public Vector3 position;
    public UnityVehicle vehiclePrefab;
    public AttachmentPrototype attachmentPrototype;
    public RamPrototype ramPrototype;
    public DropPrototype dropPrototype;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public ArmorPresentationPrototype presentationPrototype;

    public ArmorPrototype(Vector3 position, UnityVehicle vehiclePrefab, AttachmentPrototype attachmentPrototype, RamPrototype ramPrototype, DropPrototype dropPrototype, HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype, ArmorPresentationPrototype presentationPrototype) {
        this.position = position;
        this.vehiclePrefab = vehiclePrefab;
        this.attachmentPrototype = attachmentPrototype;
        this.ramPrototype = ramPrototype;
        this.dropPrototype = dropPrototype;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
        this.presentationPrototype = presentationPrototype;
    }
}
