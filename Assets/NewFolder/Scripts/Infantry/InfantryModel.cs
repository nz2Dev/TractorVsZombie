using UnityEngine;

public class InfantryModel {
    
    private readonly InfantryConfig config;

    public int Id { get; private set; }
    public int CombatId { get; set; }
    public int AvoidanceId { get; set; }
    public int PhysicsId { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public bool Grounded { get; set; }
    public int Health { get; set; }
    public float LastAttackTime { get; set; }

    public bool IsAlive => Health > 0;
    public int MaxHealth => config.maxHealth;
    public float AttackCooldown => config.attackCooldown;
    public int Damage => config.damage;
    public InfantryVisuals VisualsPrefab => config.visualsPrefab;
    public InfantryPhysicsConfig PhysicsConfig => config.physicsConfig;

    public InfantryModel(int id, Vector3 position, InfantryConfig config) {
        Id = id;
        Position = position;
        this.config = config;
    }

}