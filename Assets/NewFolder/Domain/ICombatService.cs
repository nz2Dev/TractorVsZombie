using UnityEngine;

public struct AgentState {
    public bool exploded;
    public bool projectiled;
    public int damage;
    public Vector3 damageSourcePosition;
    public int damageSourceAgentId;
}

public struct AgentInfo { 
    public int id;
    public int groupId;
    public Vector3 position;
    public float height;
}

public interface ICombatService {
    const int UnspecifiedGroupId = -1;

    int AddGroup();
    int RegisterAgent(Vector3 position, int groupId = -1, float height = 1f);
    void UnregisterAgent(int agentId);
    AgentState GetAgentState(int agentId);
    void ClearAgentState(int agentId);
    void UpdateAgentPosition(int agentId, Vector3 position);
    bool ApplyProjectileDamage(int agentId, Vector3 position, Vector3 direction, int damage);
    void ApplyExplosionDamage(int sourceAgentId, Vector3 position, float radius, int damage);
    void ApplyDirectDamage(int agentId, int targetId, int damage);
    bool GetClosestEnemyAgentInRange(int combatAgentId, float radius, out AgentInfo agentInfo, int excludeGroup = UnspecifiedGroupId);
}