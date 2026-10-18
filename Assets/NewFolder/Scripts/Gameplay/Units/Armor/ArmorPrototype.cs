
using UnityEngine;

[System.Serializable]
public struct ArmorPrototype {
    
    public Vector3 position;
    public ArmorConfig config;
    public ArmorVisuals visualsPrefab;
    public UnityVehicle vehiclePrefab;
    public AudioClip engineLoopSFX;
    public AttachmentPrototype attachmentPrototype;
    public RamEffectPrototype ramPrototype;
    public DropPrototype dropPrototype;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public WorldSpaceUI worldSpaceUIPrefab;

    public ArmorPrototype(Vector3 position, ArmorConfig config, ArmorVisuals visualsPrefab, UnityVehicle vehiclePrefab, AudioClip engineLoopSFX, AttachmentPrototype attachmentPrototype, RamEffectPrototype ramPrototype, DropPrototype dropPrototype, HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype, WorldSpaceUI worldSpaceUIPrefab) {
        this.position = position;
        this.config = config;
        this.visualsPrefab = visualsPrefab;
        this.vehiclePrefab = vehiclePrefab;
        this.engineLoopSFX = engineLoopSFX;
        this.attachmentPrototype = attachmentPrototype;
        this.ramPrototype = ramPrototype;
        this.dropPrototype = dropPrototype;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
        this.worldSpaceUIPrefab = worldSpaceUIPrefab;
    }
}
