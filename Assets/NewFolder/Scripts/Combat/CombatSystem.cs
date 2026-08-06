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
        ResolveDamage();
        UpdateLookups();
    }

    public void Destroy() {
        alieLookup.Dispose();
        foeLookup.Dispose();
    }

    private void ResolveDamage() {
        foreach (var agent in agents.Values) {
            var fatalDamage = false;
            if (agent.ReceivedDamage > 0) {
                agent.Health -= agent.ReceivedDamage;
                if (agent.Health <= 0) {
                    fatalDamage = true;
                }
            }

            agent.Output =  new CombatOutputInfo {
                damageTaken = agent.ReceivedDamage,
                damageSourcePosition = agent.DamageSourcePosition,
                wasExploded = agent.DamageByExplosion,
                explosionData = agent.ExplosionData,
                wasProjectiled = agent.DamageByProjectile,
                wasPunched = agent.DamageByPunch,
                damageWasFatal = fatalDamage
            };

            agent.ClearEvents();
        }
    }

    public int RegisterAgent(Vector3 position, CombatAgentConfig config) {
        return RegisterAgent(position, config.alie, config.maxHealth, config.collisionHeight, config.collisionRadius);
    }

    public int RegisterAgent(Vector3 position, bool alie, int maxHealth = 1, float height = 1f, float radius = .3f) {
        var agentId = idCounter++;
        var agent = new CombatAgent(agentId, alie, maxHealth, height) {
            Position = position,
            Health = maxHealth
        };
        agents[agentId] = agent;
        
        if (alie) alieCollisionsLookup.Add(agent, position, height, radius);
        else foeCollisionLookup.Add(agent, position, height, radius);
        
        return agentId;
    }

    public void UnregisterAgent(int agentId) {
        agents.Remove(agentId, out var agent);
        if (agent.Alie) alieCollisionsLookup.Remove(agent);
        else foeCollisionLookup.Remove(agent);
    }

    public void UpdateAgentPosition(int agentId, Vector3 position) {
        var agent = agents[agentId];
        agent.Position = position;
        if (agent.Alie) alieCollisionsLookup.Update(agent, position);
        else foeCollisionLookup.Update(agent, position);
    }

    public bool ApplyProjectileDamage(int agentId, Vector3 position, float testDistance, Vector3 direction, int damage) {
        return ApplyProjectileDamage(agentId, position, direction, testDistance, damage, out var _);
    }

    public bool ApplyProjectileDamage(int agentId, Vector3 position, Vector3 direction, float testDistance, int damage, out Vector3 hitDirection) {
        hitDirection = Vector3.zero;
        if (!agents.TryGetValue(agentId, out var agent)) {
            return false;
        }

        // Combat System should only check for agents collisions (obstacles is projectile source responsibilities)
        var enemyCollisionLookup = agent.Alie ? foeCollisionLookup : alieCollisionsLookup;
        if (!enemyCollisionLookup.Raycast(position, direction, testDistance, out var hitAgent, out var hitInfo)) {
            return false;
        }
        
        if (hitAgent.Id != agentId) {
            hitAgent.DamageByProjectile = true;
            hitAgent.ReceivedDamage = damage;
            hitAgent.DamageSourcePosition = position;
            hitDirection = hitInfo.normal;
        }
        return true;
    }

    public int ApplyExplosionDamage(int sourceAgentId, Vector3 position, float triggerRadius, int damage, ExplosionData explosionData) {
        var sourceAgent = agents[sourceAgentId];
        var collisionsLookup = sourceAgent.Alie ? foeCollisionLookup : alieCollisionsLookup;
        var overlapCount = collisionsLookup.Overlap(position, triggerRadius, out var results);
        int affectedCount = 0;
        for (int i = 0; i < overlapCount; i++) {
            var affectedAgent = results[i];
            if (affectedAgent.Id != sourceAgentId) {
                affectedAgent.DamageByExplosion = true;
                affectedAgent.ExplosionData = explosionData;
                affectedAgent.ReceivedDamage = damage;
                affectedAgent.DamageSourcePosition = position;
                affectedCount++;
            }            
        }
        return affectedCount;
    }

    public void ApplyDirectDamage(int agentId, int targetId, int damage) {
        var sourceAgent = agents[agentId];
        var targetAgent = agents[targetId];
        targetAgent.ReceivedDamage = damage;
        targetAgent.DamageByPunch = true;
        targetAgent.DamageSourcePosition = sourceAgent.Position;
    }

    public bool GetClosestEnemyAgentInRange(int combatAgentId, float radius, out CombatAgentInfo agentInfo) {
        var sourceAgent = agents[combatAgentId];
        var sourceAgentPosition = sourceAgent.Position;
        var enemyLookup = sourceAgent.Alie ? foeLookup : alieLookup;
        
        agentInfo = default;
        if (enemyLookup.SourceCount == 0)
            return false;
        
        var closestEnemy = enemyLookup.QueryNearest(sourceAgentPosition);
        if (Vector3.Distance(sourceAgentPosition, closestEnemy.Position) >= radius)
            return false;
        
        agentInfo = GetAgentInfo(closestEnemy);
        return true;
    }

    public CombatOutputInfo GetCombatOutput(int agentId) {
        var agent = agents[agentId];
        return agent.Output;
    }

    private CombatAgentInfo GetAgentInfo(CombatAgent agent) {
        return new CombatAgentInfo {
            id = agent.Id,
            alie = agent.Alie,
            position = agent.Position,
            height = agent.Height
        };
    }

    private void UpdateLookups() {
        alieLookup.Reset();
        foeLookup.Reset();
        
        foreach (var agent in agents.Values)
            if (agent.Alie) alieLookup.Add(agent);
            else foeLookup.Add(agent);
        
        alieLookup.Fixate();
        foeLookup.Fixate();
        if (foeLookup.SourceCount != 0 && alieLookup.SourceCount != 0) {
            JobHandle.CombineDependencies(alieLookup.ScheduleBuild(), foeLookup.ScheduleBuild())
                .Complete();
        }
    }

}