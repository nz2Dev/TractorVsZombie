using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

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
    public bool alie;
    public Vector3 position;
    public float height;
}

public class CombatService {

    internal class CombatAgent : IPositionSource {
        public int agentId;
        public bool alie;
        public float height;
        public bool projectiled;
        public bool exploded;
        public bool physicaly;
        public int damageReceived;
        public Vector3 damageSourcePosition;
        public CapsuleCollider spatialMarker;

        public Vector3 Position => spatialMarker.transform.position;

        internal void ClearState() {
            projectiled = false;
            exploded = false;
            physicaly = false;
            damageReceived = 0;
        }
    }

    private readonly int alieLayer;
    private readonly int foeLayer;
    private readonly LayerMask alieAgentsMask;
    private readonly LayerMask alieAgentsAndObstaclesMask;
    private readonly LayerMask foeAgentsMask;
    private readonly LayerMask foeAgentsAndObstaclesMask;

    public CombatService(int agentsLayer, int foeAgentsLayer, LayerMask obstaclesMask) {
        this.alieLayer = agentsLayer;
        this.foeLayer = foeAgentsLayer;
        this.alieAgentsMask = 1 << agentsLayer;
        this.alieAgentsAndObstaclesMask = alieAgentsMask | obstaclesMask;
        this.foeAgentsMask = 1 << foeAgentsLayer;
        this.foeAgentsAndObstaclesMask = foeAgentsMask | obstaclesMask;
    }

    private int idCounter;
    private readonly Dictionary<int, CombatAgent> agents = new Dictionary<int, CombatAgent>();
    private readonly Dictionary<Collider, CombatAgent> markerToAgent = new Dictionary<Collider, CombatAgent>();
    private readonly Collider[] overlapBuffer = new Collider[256];
    private readonly SpatialLookup<CombatAgent> alieLookup = new SpatialLookup<CombatAgent>(128);
    private readonly SpatialLookup<CombatAgent> foeLookup = new SpatialLookup<CombatAgent>(1024);

    public void UpdateSpatialTree() {
        alieLookup.Reset();
        foeLookup.Reset();
        
        foreach (var agent in agents.Values)
            if (agent.alie) alieLookup.Add(agent);
            else foeLookup.Add(agent);
        
        alieLookup.Fixate();
        foeLookup.Fixate();
        if (foeLookup.SourceCount != 0 && alieLookup.SourceCount != 0) {
            JobHandle.CombineDependencies(alieLookup.ScheduleBuild(), foeLookup.ScheduleBuild())
                .Complete();
        }
    }

    public int RegisterAgent(Vector3 position, bool alie, float height = 1f) {
        var agentId = idCounter++;
        var spatialMarker = CreateSpatialMarker(agentId, position, height, 0.3f, alie);
        var agent = new CombatAgent { 
            agentId = agentId, 
            alie = alie,
            spatialMarker = spatialMarker, 
        };
        
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
            exploded = agent.exploded,
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

    public bool ApplyProjectileDamage(int agentId, Vector3 position, Vector3 direction, int damage) {
        if (!agents.TryGetValue(agentId, out var agent)) {
            return false;
        }

        var enemyMask = agent.alie ? foeAgentsAndObstaclesMask : alieAgentsAndObstaclesMask;
        if (!Physics.Raycast(position, direction, out var hitInfo, 0.25f, enemyMask)) {
            return false;
        }
        
        if (markerToAgent.TryGetValue(hitInfo.collider, out var hitAgent) && hitAgent.agentId != agentId) {
            hitAgent.projectiled = true;
            hitAgent.damageReceived = damage;
            hitAgent.damageSourcePosition = position;
        }
        return true;
    }

    public int ApplyExplosionDamage(int sourceAgentId, Vector3 position, float radius, int damage) {
        var sourceAgent = agents[sourceAgentId];
        var overlapCount = Physics.OverlapSphereNonAlloc(position, radius, overlapBuffer, alieAgentsMask | foeAgentsMask);
        int affectedCount = 0;
        for (int i = 0; i < overlapCount; i++) {
            var overlapCollider = overlapBuffer[i];
            if (markerToAgent.TryGetValue(overlapCollider, out var overlapAgent) && overlapAgent.agentId != sourceAgentId) {
                overlapAgent.exploded = true;
                overlapAgent.damageReceived = damage;
                overlapAgent.damageSourcePosition = position;
                affectedCount++;
            }            
        }
        return affectedCount;
    }

    public void ApplyDirectDamage(int agentId, int targetId, int damage) {
        var sourceAgent = agents[agentId];
        var targetAgent = agents[targetId];
        targetAgent.damageReceived = damage;
        targetAgent.physicaly = true;
        targetAgent.damageSourcePosition = sourceAgent.spatialMarker.transform.position;
    }

    public bool GetClosestEnemyAgentInRange(int combatAgentId, float radius, out AgentInfo agentInfo) {
        var sourceAgent = agents[combatAgentId];
        var sourceAgentPosition = sourceAgent.Position;
        var enemyLookup = sourceAgent.alie ? foeLookup : alieLookup;
        
        agentInfo = default;
        if (enemyLookup.SourceCount == 0)
            return false;
        
        var closestEnemy = enemyLookup.QueryNearest(sourceAgentPosition);
        if (Vector3.Distance(sourceAgentPosition, closestEnemy.Position) >= radius)
            return false;
        
        agentInfo = GetAgentInfo(closestEnemy);
        return true;
    }

    private AgentInfo GetAgentInfo(CombatAgent agent) {
        return new AgentInfo {
            id = agent.agentId,
            alie = agent.alie,
            position = agent.spatialMarker.transform.position,
            height = agent.spatialMarker.height
        };
    }

    private CapsuleCollider CreateSpatialMarker(int id, Vector3 position, float height, float radius, bool alie) {
        var go = new GameObject("Combat Agent (New) " + id, typeof(CapsuleCollider));
        go.transform.position = position;
        go.layer = alie ? alieLayer : foeLayer;
        var collider = go.GetComponent<CapsuleCollider>();
        collider.isTrigger = true;
        collider.height = height;
        collider.radius = radius;
        collider.center = new Vector3(0, height * 0.5f, 0);
        return collider;
    }

}