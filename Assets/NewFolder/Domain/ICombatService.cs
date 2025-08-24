using UnityEngine;

public struct AgentState {
    public bool pushed;
    public bool projectiled;
    public int damage;
    public Vector3 damageSourcePosition;
    public int damageSourceAgentId;
}

public struct AgentInfo {
    public int id;
    public Vector3 position;
}

public struct ProjectileState {
    public bool destroyed;
    public bool hit;
}

public interface ICombatService {
    int RegisterAgent(Vector3 position);
    void UnregisterAgent(int agentId);
    AgentState GetAgentState(int agentId);
    void ClearAgentState(int agentId);
    void UpdateAgentPosition(int agentId, Vector3 position);
    void ApplyPushDamage(int agentId, Vector3 size, int damage);
    bool ApplyProjectileDamage(int agentId, Vector3 position, Vector3 direction, int damage);
    bool GetClosestEnemyAgentInRange(int combatAgentId, float radius, out AgentInfo agentInfo);
}