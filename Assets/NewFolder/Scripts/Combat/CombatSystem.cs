using System.Collections.Generic;

using Unity.Jobs;

using UnityEngine;

public class CombatSystem {

    private readonly LayerMask alieAgentsMask;
    private readonly LayerMask alieAgentsAndObstaclesMask;
    private readonly LayerMask foeAgentsMask;
    private readonly LayerMask foeAgentsAndObstaclesMask;

    private int idCounter;
    private readonly Dictionary<int, CombatAgent> agents = new Dictionary<int, CombatAgent>();
    private readonly SpatialLookup<CombatAgent> alieLookup = new SpatialLookup<CombatAgent>(128);
    private readonly CollisionLookup<CombatAgent> alieCollisionsLookup;
    private readonly SpatialLookup<CombatAgent> foeLookup = new SpatialLookup<CombatAgent>(1024);
    private readonly CollisionLookup<CombatAgent> foeCollisionLookup;

    public CombatSystem(int agentsLayer, int foeAgentsLayer, LayerMask obstaclesMask) {
        this.alieAgentsMask = 1 << agentsLayer;
        this.alieAgentsAndObstaclesMask = alieAgentsMask | obstaclesMask;
        this.foeAgentsMask = 1 << foeAgentsLayer;
        this.foeAgentsAndObstaclesMask = foeAgentsMask | obstaclesMask;
        alieCollisionsLookup = new CollisionLookup<CombatAgent>(agentsLayer, 64);
        foeCollisionLookup = new CollisionLookup<CombatAgent>(foeAgentsLayer, 512);
    }

    public void Update() {
        UpdateLookups();
    }

    private void UpdateLookups() {
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
        var agent = new CombatAgent { agentId = agentId, position = position, alie = alie, };
        agents[agentId] = agent;
        
        if (alie) alieCollisionsLookup.Add(agent, position, height, .3f);
        else foeCollisionLookup.Add(agent, position, height, .3f);
        
        return agentId;
    }

    public void UnregisterAgent(int agentId) {
        agents.Remove(agentId, out var agent);
        if (agent.alie) alieCollisionsLookup.Remove(agent);
        else foeCollisionLookup.Remove(agent);
    }

    public CombatOutputInfo GetAgentState(int agentId) {
        var agent = agents[agentId];
        return new CombatOutputInfo {
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
        var agent = agents[agentId];
        agent.position = position;
        if (agent.alie) alieCollisionsLookup.Update(agent, position);
        else foeCollisionLookup.Update(agent, position);
    }

    public bool ApplyProjectileDamage(int agentId, Vector3 position, Vector3 direction, int damage) {
        if (!agents.TryGetValue(agentId, out var agent)) {
            return false;
        }

        // Combat System should only check for agents collisions (obstacles is projectile source responsibilities)
        var enemyCollisionLookup = agent.alie ? foeCollisionLookup : alieCollisionsLookup;
        if (!enemyCollisionLookup.Raycast(position, direction, 0.25f, out var hitAgent)) {
            return false;
        }
        
        if (hitAgent.agentId != agentId) {
            hitAgent.projectiled = true;
            hitAgent.damageReceived = damage;
            hitAgent.damageSourcePosition = position;
        }
        return true;
    }

    public int ApplyExplosionDamage(int sourceAgentId, Vector3 position, float radius, int damage) {
        var sourceAgent = agents[sourceAgentId];
        var collisionsLookup = sourceAgent.alie ? foeCollisionLookup : alieCollisionsLookup;
        var overlapCount = collisionsLookup.Overlap(position, radius, out var results);
        int affectedCount = 0;
        for (int i = 0; i < overlapCount; i++) {
            var affectedAgent = results[i];
            if (affectedAgent.agentId != sourceAgentId) {
                affectedAgent.exploded = true;
                affectedAgent.damageReceived = damage;
                affectedAgent.damageSourcePosition = position;
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
        targetAgent.damageSourcePosition = sourceAgent.position;
    }

    public bool GetClosestEnemyAgentInRange(int combatAgentId, float radius, out CombatAgentInfo agentInfo) {
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

    private CombatAgentInfo GetAgentInfo(CombatAgent agent) {
        return new CombatAgentInfo {
            id = agent.agentId,
            alie = agent.alie,
            position = agent.position,
            height = agent.height
        };
    }

}