using System;
using System.Collections.Generic;

using UnityEngine;

public class CombatAgent {
    public int agentId;
    public bool pushed;
    public int damageReceived;
    public Vector3 damageSourcePosition;
    public SphereCollider spatialMarker;

    internal void ClearState() {
        pushed = false;
        damageReceived = 0;
    }
}

public class CombatService : ICombatService {

    private readonly int layer;
    private readonly LayerMask queryMask;

    public CombatService(int layer) {
        this.layer = layer;
        this.queryMask = 1 << layer;
    }

    private int idCounter;
    private readonly Dictionary<int, CombatAgent> agents = new Dictionary<int, CombatAgent>();
    private readonly Dictionary<Collider, CombatAgent> markerToAgent = new Dictionary<Collider, CombatAgent>();
    
    private readonly Collider[] overlapBuffer = new Collider[64];

    public int RegisterAgent(Vector3 position) {
        var agentId = idCounter++;
        var spatialMarker = CreateSpatialMarker(agentId, position, 0.3f);
        var agent = new CombatAgent { agentId = agentId, spatialMarker = spatialMarker, };
        markerToAgent[spatialMarker] = agent;
        agents[agentId] = agent;
        return agentId;
    }

    public void UnregisterAgent(int agentId) {
        var agent = agents[agentId];
        markerToAgent.Remove(agent.spatialMarker);
        GameObject.Destroy(agent.spatialMarker.gameObject);
        agents.Remove(agentId);
    }

    public AgentState GetAgentState(int agentId) {
        var agent = agents[agentId];
        return new AgentState {
            pushed = agent.pushed,
            projectiled = false,
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

    public int RegisterProjectile(int parentAgentId, Vector3 position, int damage) {
        throw new NotImplementedException();
    }

    public void UpdateProjectile(int parentAgentId, int projectileId, Vector3 position) {
        throw new NotImplementedException();
    }

    public int GetDestroyedProjectilesEventsCount(int parentAgentId) {
        throw new NotImplementedException();
    }

    public int GetDestroyedProjectileIndex(int parentAgentId, int destroyedEventIndex) {
        throw new NotImplementedException();
    }

    private SphereCollider CreateSpatialMarker(int id, Vector3 position, float radius) {
        var go = new GameObject("Combat Agent (New) " + id, typeof(SphereCollider));
        go.transform.position = position;
        go.layer = layer;
        var collider = go.GetComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = radius;
        return collider;
    }

    private bool CheckRegisteredMarker(Collider overlapCollider) {
        if (!markerToAgent.ContainsKey(overlapCollider)) {
            Debug.LogWarning($"overlapping collider {overlapCollider} that has no entry in markerToAgent");
            return false;
        }
        return true;
    }

}