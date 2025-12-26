using System;

using UnityEngine;

[Serializable]
public struct InfantryPhysicsConfig {
    public float height;
    public float radius;
}

[CreateAssetMenu(fileName = "InfantryConfig", menuName = "InfantryConfig", order = 0)]
public class InfantryConfig : ScriptableObject {
    public int maxHealth = 5;
    public float attackCooldown = 1;
    public int damage = 1;
    public InfantryVisuals visualsPrefab;
    public InfantryPhysicsConfig physicsConfig = new InfantryPhysicsConfig { height = 0.5f, radius = 0.15f};
    public AgentAvoidanceConfig agentAvoidanceConfig;
}