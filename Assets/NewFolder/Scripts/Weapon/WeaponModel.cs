using Combat;

using UnityEngine;

public class WeaponModel {
    
    public int Id { get; }
    public CombatId CombatId { get; }
    public WeaponConfig Config { get; }

    public WeaponModel(int id, CombatId combatId, WeaponConfig config) {
        Id = id;
        CombatId = combatId;
        Config = config;
    }

    public Vector3 Position { get; set; }
    public BallisticPrototype BallisticPrototype { get; set; }
    public Vector3 BallisticLaunchOffset { get; set; }
    public Vector3 AimPoint { get; set; }
    public float LastShootTime { get; set; }
    
}