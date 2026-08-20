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
}
