using UnityEngine;

public class InfantryModel {
    
    private readonly InfantryConfig config;

    public int Id { get; private set; }
    public int CombatId { get; set; }
    public int BodyId { get; set; }
    public BodyState BodyState { get; set; }
    
    public bool IsDead { get; set; }
    public float LastAttackTime { get; set; }

    public float AttackCooldown => config.attackCooldown;
    public int Damage => config.damage;
    public BodyConfig BodyConfig => config.bodyConfig;
    public int MaxHealthConfig => config.maxHealth;
    public InfantryVisuals VisualsPrefab => config.visualsPrefab;
    public AgentAvoidanceConfig AgentAvoidanceConfig => config.agentAvoidanceConfig;

    public InfantryModel(int id, InfantryConfig config) {
        Id = id;
        this.config = config;
    }

}