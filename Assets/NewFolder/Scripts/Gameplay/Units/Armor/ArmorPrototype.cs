
using UnityEngine;

[System.Serializable]
public struct ArmorPrototype {
    
    public Vector3 position;
    public VehiclePrototype vehiclePrototype;
    public AttachmentPrototype attachmentPrototype;
    public RamPrototype ramPrototype;
    public DropPrototype dropPrototype;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public ArmorPresentationPrototype presentationPrototype;

    public ArmorPrototype(Vector3 position, VehiclePrototype vehiclePrototype, AttachmentPrototype attachmentPrototype, RamPrototype ramPrototype, DropPrototype dropPrototype, HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype, ArmorPresentationPrototype presentationPrototype) {
        this.position = position;
        this.vehiclePrototype = vehiclePrototype;
        this.attachmentPrototype = attachmentPrototype;
        this.ramPrototype = ramPrototype;
        this.dropPrototype = dropPrototype;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
        this.presentationPrototype = presentationPrototype;
    }
}
