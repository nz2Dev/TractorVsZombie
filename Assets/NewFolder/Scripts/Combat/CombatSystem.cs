using System;
using System.Collections.Generic;

using Unity.Jobs;

using UnityEngine;

public class CombatSystem {

    private static ReservedLayerCode MapFactionToLayerCode(bool alie) => alie ? ReservedLayerCode.First : ReservedLayerCode.Second;

    private readonly RaycastService raycastService;

    private int idCounter;
    private readonly Dictionary<int, CombatAgent> agents = new Dictionary<int, CombatAgent>();
    private readonly SpatialLookup<CombatAgent> alieLookup = new SpatialLookup<CombatAgent>(128);
    private readonly SpatialLookup<CombatAgent> foeLookup = new SpatialLookup<CombatAgent>(1024);

    public CombatSystem(RaycastService raycastService) {
        this.raycastService = raycastService;
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

    public int RegisterAgent(Vector3 position, CombatAgentPrototype prototype) {
        var agentId = idCounter++;
        var agent = new CombatAgent(agentId, prototype.alie, prototype.config, prototype.markerPrefab.Height);
        agent.Position = position;
        agent.Health = agent.Config.maxHealth;
        agents[agentId] = agent;
        var layerCode = MapFactionToLayerCode(agent.Alie);
        raycastService.RegisterMarker(agentId, position, prototype.markerPrefab, layerCode);
        return agentId;
    }

    public void UnregisterAgent(int agentId) {
        agents.Remove(agentId, out var agent);
        raycastService.UnregisterMarker(agentId);
    }

    public void UpdateAgentPosition(int agentId, Vector3 position) {
        var agent = agents[agentId];
        agent.Position = position;
        raycastService.UpdateMarker(agentId, position);
    }

    public bool ApplyProjectileDamage(int agentId, Vector3 position, float testDistance, Vector3 direction, int damage) {
        return ApplyProjectileDamage(agentId, position, direction, testDistance, damage, out var _);
    }

    public bool ApplyProjectileDamage(int agentId, Vector3 position, Vector3 direction, float testDistance, int damage, out Vector3 hitDirection) {
        hitDirection = Vector3.zero;
        if (!agents.TryGetValue(agentId, out var agent)) {
            return false;
        }

        var enemyFaction = !agent.Alie;
        var enemyLayerCode = MapFactionToLayerCode(enemyFaction);
        if (!raycastService.Raycast(new Ray(position, direction), testDistance, enemyLayerCode, out var hitAgentId, out var hitInfo)) {
            return false;
        }
        
        var hitAgent = agents[hitAgentId];
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
        var enemyFaction = !sourceAgent.Alie;
        var enemyLayerCode = MapFactionToLayerCode(enemyFaction);
        
        var overlapCount = raycastService.Overlap(position, triggerRadius, enemyLayerCode, out var agentIdResults);
        int affectedCount = 0;
        for (int i = 0; i < overlapCount; i++) {
            var affectedAgent = agents[agentIdResults[i]];
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