using UnityEngine;

public struct InfantryPrototype {
    public Vector3 position;
    public InfantryConfig config;
    public InfantryVisuals visualsPrefab;
    public RewardPrototype rewardPrototype;
    public RagdollBody physicsBodyPrefab;
    public CombatAgentPrototype combatAgentPrototype;
    public AgentAvoidanceConfig agentAvoidanceConfig;
}
