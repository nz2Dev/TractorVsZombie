using System;
using System.Collections.Generic;

using UnityEngine;

public class CombatAgent {

    public int agentId;
    public float physicalDamage;
    public float damageReceived;
    public SphereCollider spatialMarker;

}

public class CombatService {

    private readonly int layer;
    private readonly LayerMask queryMask;

    public CombatService(int layer) {
        this.layer = layer;
        this.queryMask = 1 << layer;
    }

    private readonly Dictionary<int, CombatAgent> agents = new Dictionary<int, CombatAgent>();
    private readonly Dictionary<Collider, CombatAgent> markerToAgent = new Dictionary<Collider, CombatAgent>();
    
    private readonly Collider[] overlapBuffer = new Collider[64];

    public int RegisterCombatant(float radius, Vector3 position, float physicalDamage) {
        var agentId = agents.Count;
        var collider = CreateSpatialMarker(position, radius);
        var agent = new CombatAgent {
            agentId = agentId,
            physicalDamage = physicalDamage,
            spatialMarker = collider
        };
        
        markerToAgent[collider] = agent;
        agents[agentId] = agent;
        return agentId;
    }

    public void UnregisterAgent(int agentId) {
        var agent = agents[agentId];
        markerToAgent.Remove(agent.spatialMarker);
        GameObject.Destroy(agent.spatialMarker);
        agents.Remove(agentId);
    }

    public void UpdateAgentPosition(int agentId, Vector3 position) {
        var agent = agents[agentId];   
        agent.spatialMarker.transform.position = position;
    }

    public void ApplyDirectDamage(int sourceId, int targetId) {
        var sourceAgent = agents[sourceId];
        var targetAgent = agents[targetId];
        targetAgent.damageReceived += sourceAgent.physicalDamage;
    }

    public bool ApplyPushDamage(int sourceId, Vector3 areaSize) {
        var sourceAgent = agents[sourceId];
        var overlapCount = Physics.OverlapBoxNonAlloc(sourceAgent.spatialMarker.transform.position, areaSize * 0.5f, overlapBuffer, Quaternion.identity, queryMask);
        int damageCount = 0;
        for (int i = 0; i < overlapCount; i++) {
            var overlapCollider = overlapBuffer[i];
            var overlapAgent = markerToAgent[overlapCollider];
            if (overlapAgent.agentId == sourceId) 
                continue;

            overlapAgent.damageReceived += sourceAgent.physicalDamage;
            damageCount++;
        }

        return damageCount > 0;
    }

    public CombatAgent GetState(int agentId) {
        return agents[agentId];
    }

    private SphereCollider CreateSpatialMarker(Vector3 position, float radius) {
        var go = new GameObject("Combat Agent (New)", typeof(SphereCollider));
        go.transform.position = position;
        go.layer = layer;
        var collider = go.GetComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = radius;
        return collider;
    }

}