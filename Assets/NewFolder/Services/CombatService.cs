using System;
using System.Collections.Generic;

using UnityEngine;

public class CombatAgent {
    public int agentId;
    public int groupId;
    public float height;
    public bool pushed;
    public bool projectiled;
    public int damageReceived;
    public Vector3 damageSourcePosition;
    public CapsuleCollider spatialMarker;

    internal void ClearState() {
        pushed = false;
        damageReceived = 0;
    }
}

public class CombatService : ICombatService {

    private readonly int layer;
    private readonly LayerMask queryMask;

    private readonly int registeredDefaultGroupId;

    public CombatService(int layer) {
        this.layer = layer;
        this.queryMask = 1 << layer;

        registeredDefaultGroupId = AddGroup();
    }

    private int idCounter;
    private int groupIdCounter = 1;
    private readonly Dictionary<int, CombatAgent> agents = new Dictionary<int, CombatAgent>();
    private readonly Dictionary<int, List<int>> agentToGroupRegistry = new ();
    private readonly Dictionary<Collider, CombatAgent> markerToAgent = new Dictionary<Collider, CombatAgent>();
    
    private readonly Collider[] overlapBuffer = new Collider[64];

    public int AddGroup() {
        var nextGroupId = groupIdCounter++;
        var agentsList = new List<int>(128);
        agentToGroupRegistry[nextGroupId] = agentsList;
        return nextGroupId;
    }

    public int RegisterAgent(Vector3 position, int groupId = -1, float height = 1f) {
        var agentId = idCounter++;
        var spatialMarker = CreateSpatialMarker(agentId, position, height, 0.3f);
        var agent = new CombatAgent { 
            agentId = agentId, 
            groupId = groupId,
            spatialMarker = spatialMarker, 
        };
        
        markerToAgent[spatialMarker] = agent;
        agents[agentId] = agent;

        var groupRegistry = GetGroupRegistry(groupId);
        groupRegistry.Add(agentId);
        
        return agentId;
    }

    public void UnregisterAgent(int agentId) {
        var agent = agents[agentId];
        markerToAgent.Remove(agent.spatialMarker);
        GameObject.Destroy(agent.spatialMarker.gameObject);
        agents.Remove(agentId);
        var agentGroupRegistry = GetAgentGroupRegistry(agentId);
        agentGroupRegistry.Remove(agentId);
    }

    public AgentState GetAgentState(int agentId) {
        var agent = agents[agentId];
        return new AgentState {
            pushed = agent.pushed,
            projectiled = agent.projectiled,
            damage = agent.damageReceived,
            damageSourceAgentId = -1,
            damageSourcePosition = agent.damageSourcePosition
        };
    }

    public void ClearAgentState(int agentId) {
        var agent = agents[agentId];
        agent.ClearState();
    }

    public void UpdateAgentPosition(int agentId, Vector3 position) {
        agents[agentId].spatialMarker.transform.position = position;
    }

    public void ApplyPushDamage(int agentId, Vector3 size, int damage) {
        var sourceAgent = agents[agentId];
        var overlapCount = Physics.OverlapBoxNonAlloc(sourceAgent.spatialMarker.transform.position, size * 0.5f, overlapBuffer, Quaternion.identity, queryMask);
        
        for (int i = 0; i < overlapCount; i++) {
            var overlapCollider = overlapBuffer[i];
            if (markerToAgent.TryGetValue(overlapCollider, out var overlapAgent) && overlapAgent.agentId != agentId) {
                overlapAgent.pushed = true;
                overlapAgent.damageReceived = damage;
                overlapAgent.damageSourcePosition = sourceAgent.spatialMarker.transform.position;
            }            
        }
    }

    public bool ApplyProjectileDamage(int agentId, Vector3 position, Vector3 direction, int damage) {
        if (Physics.Raycast(position, direction, out var hitInfo, 0.25f, queryMask)) {
            if (markerToAgent.TryGetValue(hitInfo.collider, out var hitAgent) && hitAgent.agentId != agentId) {
                hitAgent.projectiled = true;
                hitAgent.damageReceived = damage;
                hitAgent.damageSourcePosition = position;
                return true;
            }
        }
        return false;
    }

    public bool GetClosestEnemyAgentInRange(int combatAgentId, float radius, out AgentInfo agentInfo) {
        var sourceAgent = agents[combatAgentId];
        var sourceAgentPosition = sourceAgent.spatialMarker.transform.position;
        var overlapCount = Physics.OverlapSphereNonAlloc(sourceAgentPosition, radius, overlapBuffer);
        CombatAgent closestAgent = null;
        float closestDistance = float.PositiveInfinity;
        for (int i = 0; i < overlapCount; i++) {
            if (!markerToAgent.TryGetValue(overlapBuffer[i], out var overlapAgent) || overlapAgent.agentId == combatAgentId)
                continue;
            
            var overlapAgentPosition = overlapAgent.spatialMarker.transform.position;
            var indexDistance = Vector3.Distance(overlapAgentPosition, sourceAgentPosition);
            if (indexDistance < closestDistance) {
                closestDistance = indexDistance;
                closestAgent = overlapAgent;
            }
        }
        
        if (closestAgent != null) {
            agentInfo = GetAgentInfo(closestAgent);
            return true;
        }

        agentInfo = default;
        return false;
    }

    private AgentInfo GetAgentInfo(CombatAgent agent) {
        return new AgentInfo {
            id = agent.agentId,
            groupId = agent.groupId,
            position = agent.spatialMarker.transform.position,
            height = agent.spatialMarker.height
        };
    }

    private List<int> GetGroupRegistry(int groupId) {
        var defaultCheckedGroupId = groupId == -1 ? registeredDefaultGroupId : groupId;
        return agentToGroupRegistry[defaultCheckedGroupId];
    }

    private List<int> GetAgentGroupRegistry(int agentId) {
        var agent = agents[agentId];
        return GetGroupRegistry(agent.groupId);
    }

    private CapsuleCollider CreateSpatialMarker(int id, Vector3 position, float height, float radius) {
        var go = new GameObject("Combat Agent (New) " + id, typeof(CapsuleCollider));
        go.transform.position = position;
        go.layer = layer;
        var collider = go.GetComponent<CapsuleCollider>();
        collider.isTrigger = true;
        collider.height = height;
        collider.radius = radius;
        collider.center = new Vector3(0, height * 0.5f, 0);
        return collider;
    }

}