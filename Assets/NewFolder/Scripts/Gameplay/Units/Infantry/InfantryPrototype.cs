using Combat;

using UnityEngine;

public struct InfantryPrototype {
    public Vector3 position;
    public InfantryConfig config;
    public InfantryVisuals visualsPrefab;
    public RewardPrototype rewardPrototype;

    public RagdollBody physicsBodyPrefab;
    public RaycastMarker raycastMarkerPrefab;
    public CombatPrototype combatPrototype;
    public AgentAvoidanceConfig agentAvoidanceConfig;
}
