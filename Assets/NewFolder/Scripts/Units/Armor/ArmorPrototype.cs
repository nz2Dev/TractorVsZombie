using Combat;

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
    public CombatPrototype combatPrototype;
    public RaycastMarker raycastMarkerPrefab;

    public ArmorPrototype(Vector3 position, ArmorConfig config, ArmorVisuals visualsPrefab, UnityVehicle vehiclePrefab, AudioClip engineLoopSFX, WeaponPrototype localWeaponPrototype, RamEffectPrototype ramPrototype, RewardPrototype rewardPrototype, CombatPrototype combatPrototype, RaycastMarker raycastMarkerPrefab) {
        this.position = position;
        this.config = config;
        this.visualsPrefab = visualsPrefab;
        this.vehiclePrefab = vehiclePrefab;
        this.engineLoopSFX = engineLoopSFX;
        this.localWeaponPrototype = localWeaponPrototype;
        this.ramPrototype = ramPrototype;
        this.rewardPrototype = rewardPrototype;
        this.combatPrototype = combatPrototype;
        this.raycastMarkerPrefab = raycastMarkerPrefab;
    }
}
