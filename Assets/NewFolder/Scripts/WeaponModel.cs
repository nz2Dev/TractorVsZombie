using UnityEngine;

public class WeaponModel {
    
    private readonly WeaponConfig config;

    public WeaponModel(int id, int combatId, Vector3 position, WeaponConfig config) {
        Id = id;
        CombatId = combatId;
        Position = position;
        this.config = config;
    }

    public int Id { get; private set; }
    public int CombatId { get; private set; }
    public Vector3 Position { get; set; }
    public Vector3 AimPoint { get; set; }
    public float LastShootTime { get; set; }
    
    public AimConfig AimConfig => config.aimConfig;
    public BallisticConfig BallisticConfig => config.ballisticConfig;  
    public WeaponVisuals VisualsPrefab => config.visualsPrefab;
    public Vector3 LaunchPoint => config.launchPoint;
    public float CooldownSec => config.cooldownSec;
    
}