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
    public int groupId;
    public Vector3 position;
    public float height;
}

public class CombatService {

    internal class CombatAgent {
        public int agentId;
        public int groupId;
        public float height;
        public bool projectiled;
        public bool exploded;
        public bool physicaly;
        public int damageReceived;
        public Vector3 damageSourcePosition;
        public CapsuleCollider spatialMarker;
        public CombatAgent[] closestAgents = new CombatAgent[25];
        public int closestAgentsCount;

        internal void ClearState() {
            projectiled = false;
            exploded = false;
            physicaly = false;
            damageReceived = 0;
        }
    }

    const int UnspecifiedGroupId = -1;

    private readonly int layer;
    private readonly LayerMask agentsMask;
    private readonly LayerMask agentsAndObstaclesMask;

    private readonly int registeredDefaultGroupId;

    public CombatService(int agentsLayer, LayerMask obstaclesMask) {
        this.layer = agentsLayer;
        this.agentsMask = 1 << agentsLayer;
        this.agentsAndObstaclesMask = agentsMask | obstaclesMask;

        registeredDefaultGroupId = AddGroup();
    }

    private int idCounter;
    private int groupIdCounter = 1;
    private readonly Dictionary<int, CombatAgent> agents = new Dictionary<int, CombatAgent>();
    private readonly Dictionary<int, List<int>> agentToGroupRegistry = new ();
    private readonly Dictionary<Collider, CombatAgent> markerToAgent = new Dictionary<Collider, CombatAgent>();
    
    private readonly Collider[] overlapBuffer = new Collider[256];

    private CombatAgent[] agentsAsPoints = new CombatAgent[512];
    private int agentsCalculated;

    public void UpdateSpatialTree() {
        var points = new NativeArray<float3>(agents.Count, Allocator.TempJob);
        agentsCalculated = agents.Count;

        int index = 0;
        foreach (var agent in agents.Values) {
            points[index] = agent.spatialMarker.transform.position;
            agentsAsPoints[index] = agent;
            index++;
        }
        
        var kdTreeContainer = new KnnContainer(points, false, Allocator.TempJob);
        var rebuildJob = new KnnRebuildJob(kdTreeContainer);
        rebuildJob.Schedule().Complete();

        var kNearest = Mathf.Min(points.Length, 25);
        var results = new NativeArray<int>(points.Length * kNearest, Allocator.TempJob);
        var queryPositions = new NativeArray<float3>(points, Allocator.TempJob);

        var batchQueryJob = new QueryKNearestBatchJob(kdTreeContainer, queryPositions, results);
        batchQueryJob.ScheduleBatch(queryPositions.Length, queryPositions.Length / 32).Complete();

        for (int i = 0; i < agentsCalculated; i++) {
            var agent = agentsAsPoints[i];
            agent.closestAgentsCount = kNearest;
            for (int j = 0; j < kNearest; j++) {
                var agentIndex = results[i * kNearest + j];
                var nextAgent = agentsAsPoints[agentIndex];
                agent.closestAgents[j] = nextAgent;    
            }
        }

        points.Dispose();
        kdTreeContainer.Dispose();
        results.Dispose();
        queryPositions.Dispose();
    }

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
        var agentGroupRegistry = GetGroupRegistry(agent.groupId);
        agentGroupRegistry.Remove(agentId);
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
        if (!Physics.Raycast(position, direction, out var hitInfo, 0.25f, agentsAndObstaclesMask)) {
            return false;
        }
        
        if (markerToAgent.TryGetValue(hitInfo.collider, out var hitAgent) && hitAgent.agentId != agentId) {
            hitAgent.projectiled = true;
            hitAgent.damageReceived = damage;
            hitAgent.damageSourcePosition = position;
        }
        return true;
    }

    public void ApplyExplosionDamage(int sourceAgentId, Vector3 position, float radius, int damage) {
        var sourceAgent = agents[sourceAgentId];
        var overlapCount = Physics.OverlapSphereNonAlloc(position, radius, overlapBuffer, agentsMask);
        
        for (int i = 0; i < overlapCount; i++) {
            var overlapCollider = overlapBuffer[i];
            if (markerToAgent.TryGetValue(overlapCollider, out var overlapAgent) && overlapAgent.agentId != sourceAgentId) {
                overlapAgent.exploded = true;
                overlapAgent.damageReceived = damage;
                overlapAgent.damageSourcePosition = position;
            }            
        }
    }

    public void ApplyDirectDamage(int agentId, int targetId, int damage) {
        var sourceAgent = agents[agentId];
        var targetAgent = agents[targetId];
        targetAgent.damageReceived = damage;
        targetAgent.physicaly = true;
        targetAgent.damageSourcePosition = sourceAgent.spatialMarker.transform.position;
    }

    public bool GetClosestEnemyAgentInRange(int combatAgentId, float radius, out AgentInfo agentInfo, int excludeGroup = UnspecifiedGroupId) {
        var sourceAgent = agents[combatAgentId];
        var sourceAgentPosition = sourceAgent.spatialMarker.transform.position;
        bool checkGroup = excludeGroup != UnspecifiedGroupId;
        CombatAgent closestAgent = null;

        for (int i = sourceAgent.closestAgentsCount - 1; i >= 0; i--) {
            var nextAgent = sourceAgent.closestAgents[i];
            if ((checkGroup && nextAgent.groupId == excludeGroup)
                || nextAgent.agentId == combatAgentId)
                continue;
            
            if (Vector3.Distance(nextAgent.spatialMarker.transform.position, sourceAgentPosition) < radius)
                closestAgent = nextAgent;
            
            break;
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