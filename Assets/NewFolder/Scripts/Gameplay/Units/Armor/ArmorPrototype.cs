
using UnityEngine;

[System.Serializable]
public struct ArmorPrototype {
    
    public Vector3 position;
    public ArmorConfig config;
    public ArmorVisuals visualsPrefab;
    public UnityVehicle vehiclePrefab;
    public AudioClip engineLoopSFX;
    public WeaponPrototype localWeaponPrototype;
    public RamEffectPrototype ramPrototype;
    public RewardPrototype rewardPrototype;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public WorldSpaceUI worldSpaceUIPrefab;

    public ArmorPrototype(Vector3 position, ArmorConfig config, ArmorVisuals visualsPrefab, UnityVehicle vehiclePrefab, AudioClip engineLoopSFX, WeaponPrototype localWeaponPrototype, RamEffectPrototype ramPrototype, RewardPrototype rewardPrototype, HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype, WorldSpaceUI worldSpaceUIPrefab) {
        this.position = position;
        this.config = config;
        this.visualsPrefab = visualsPrefab;
        this.vehiclePrefab = vehiclePrefab;
        this.engineLoopSFX = engineLoopSFX;
        this.localWeaponPrototype = localWeaponPrototype;
        this.ramPrototype = ramPrototype;
        this.rewardPrototype = rewardPrototype;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
        this.worldSpaceUIPrefab = worldSpaceUIPrefab;
    }
}
