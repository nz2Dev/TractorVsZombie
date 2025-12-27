using UnityEngine;

public class InfantryModel {
    
    private readonly InfantryConfig config;

    public int Id { get; private set; }
    public int CombatId { get; set; }
    public int BodyId { get; set; }
    public BodyState BodyState { get; set; }
    
    public int Health { get; set; }
    public float LastAttackTime { get; set; }

    public bool IsAlive => Health > 0;
    public int MaxHealth => config.maxHealth;
    public float AttackCooldown => config.attackCooldown;
    public int Damage => config.damage;
    public BodyConfig BodyConfig => config.bodyConfig;

    public InfantryModel(int id, InfantryConfig config) {
        Id = id;
        this.config = config;
    }

}