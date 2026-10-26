
using UnityEngine;

public struct InfantryPrototype {
    public Vector3 position;
    public InfantryConfig config;
    public InfantryVisuals visualsPrefab;
    public DropPrototype dropPrototype;

    public RagdollBody physicsBodyPrefab;
    public HitboxPrototype hitboxPrototype;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public AgentAvoidanceConfig agentAvoidanceConfig;
    public CombatPrototype combatPrototype;
}
