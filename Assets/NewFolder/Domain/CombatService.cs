using System;
using System.Collections.Generic;
using System.Linq;

using KNN;
using KNN.Jobs;

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

    const int kNearestQueryMax = 5;

    internal class CombatAgent {
        public int agentId;
        public bool alie;
        public float height;
        public bool projectiled;
        public bool exploded;
        public bool physicaly;
        public int damageReceived;
        public Vector3 damageSourcePosition;
        public CapsuleCollider spatialMarker;
        public CombatAgent[] closestFoes = new CombatAgent[kNearestQueryMax];
        public int closestFoesCount;

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
    private readonly List<int> alies = new (512);
    private readonly List<int> foes = new (1024);
    
    private readonly Collider[] overlapBuffer = new Collider[256];

    private CombatAgent[] agentsAsAliePoints = new CombatAgent[64];
    private CombatAgent[] agentsAsFoePoints = new CombatAgent[1024];

    public void UpdateSpatialTree() {
        if (foes.Count == 0 || alies.Count == 0)
            return;

        var foesPoints = new NativeArray<float3>(foes.Count, Allocator.TempJob);
        var aliesPoints = new NativeArray<float3>(alies.Count, Allocator.TempJob);

        int alieIndex = 0;
        int foeIndex = 0;
        foreach (var agent in agents.Values) {
            if (agent.alie) {
                aliesPoints[alieIndex] = agent.spatialMarker.transform.position;
                agentsAsAliePoints[alieIndex] = agent;
                alieIndex++;
            } else {
                foesPoints[foeIndex] = agent.spatialMarker.transform.position;
                agentsAsFoePoints[foeIndex] = agent;
                foeIndex++;
            }
        }
        
        var foeKdTreeContainer = new KnnContainer(foesPoints, false, Allocator.TempJob);
        var foeRebuildHandle = new KnnRebuildJob(foeKdTreeContainer).Schedule();

        var alieKdTreeContainer = new KnnContainer(aliesPoints, false, Allocator.TempJob);
        var alieRebuildHandle = new KnnRebuildJob(alieKdTreeContainer).Schedule();
        JobHandle.CombineDependencies(alieRebuildHandle, foeRebuildHandle).Complete();

        var kNearestFoes = Mathf.Min(foesPoints.Length, kNearestQueryMax);
        var foesResults = new NativeArray<int>(aliesPoints.Length * kNearestFoes, Allocator.TempJob);
        var alieQueryPositions = new NativeArray<float3>(aliesPoints, Allocator.TempJob);

        var foeBatchQueryJob = new QueryKNearestBatchJob(foeKdTreeContainer, alieQueryPositions, foesResults);
        foeBatchQueryJob.ScheduleBatch(alieQueryPositions.Length, Mathf.Min(alieQueryPositions.Length / 32, 16)).Complete();

        for (int i = 0; i < aliesPoints.Length; i++) {
            var agent = agentsAsAliePoints[i];
            agent.closestFoesCount = kNearestFoes;
            for (int j = 0; j < kNearestFoes; j++) {
                var agentIndex = foesResults[i * kNearestFoes + j];
                var nextAgent = agentsAsFoePoints[agentIndex];
                agent.closestFoes[j] = nextAgent;    
            }
        }

        var KNearestAlies = Mathf.Min(aliesPoints.Length, kNearestQueryMax);
        var aliesResults = new NativeArray<int>(foesPoints.Length * KNearestAlies, Allocator.TempJob);
        var foesQueryPositions = new NativeArray<float3>(foesPoints, Allocator.TempJob);

        var aliesBatchQueryJob = new QueryKNearestBatchJob(alieKdTreeContainer, foesQueryPositions, aliesResults);
        aliesBatchQueryJob.ScheduleBatch(foesQueryPositions.Length, foesQueryPositions.Length / 32).Complete();

        for (int i = 0; i < foesPoints.Length; i++) {
            var agent = agentsAsFoePoints[i];
            agent.closestFoesCount = KNearestAlies;
            for (int j = 0; j < KNearestAlies; j++) {
                var agentIndex = aliesResults[i * KNearestAlies + j];
                var nextAgent = agentsAsAliePoints[agentIndex];
                agent.closestFoes[j] = nextAgent;
            }
        }

        foesPoints.Dispose();
        aliesPoints.Dispose();
        foeKdTreeContainer.Dispose();
        alieKdTreeContainer.Dispose();
        foesResults.Dispose();
        aliesResults.Dispose();
        alieQueryPositions.Dispose();
        foesQueryPositions.Dispose();
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
        
        if (alie) alies.Add(agentId);
        else foes.Add(agentId);
        
        return agentId;
    }

    public void UnregisterAgent(int agentId) {
        var agent = agents[agentId];
        
        if (agent.alie) alies.Remove(agentId);
        else foes.Remove(agentId);
            
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
        var sourceAgentPosition = sourceAgent.spatialMarker.transform.position;
        CombatAgent closestFoe = null;

        for (int i = sourceAgent.closestFoesCount - 1; i >= 0; i--) {
            var nextAgent = sourceAgent.closestFoes[i];
            if (nextAgent.agentId == combatAgentId)
                continue;
            
            if (Vector3.Distance(nextAgent.spatialMarker.transform.position, sourceAgentPosition) < radius)
                closestFoe = nextAgent;
            
            break;
        }
        
        if (closestFoe != null) {
            agentInfo = GetAgentInfo(closestFoe);
            return true;
        }

        agentInfo = default;
        return false;
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