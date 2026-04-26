using UnityEngine;

[System.Serializable]
public struct ArmorPrototype {
    public Vector3 position;
    public ArmorConfig config;
    public ArmorVisuals visualsPrefab;
    public VehiclePhysics physicsPrefab;
    public AudioClip engineLoopSFX;
    public WeaponPrototype localWeaponPrototype;
    public LoadoutPrototype rewardLoadoutPrototype;
    public RamEffectPrototype ramPrototype;
}
