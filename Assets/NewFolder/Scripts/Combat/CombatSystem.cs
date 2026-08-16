using System;
using System.Collections.Generic;

using UnityEngine;

public class CombatSystem {

    private static ReservedLayerCode MapFactionToLayerCode(bool alie) => alie ? ReservedLayerCode.First : ReservedLayerCode.Second;
    private static ProximityService.Layer MapFactionToProximityLayer(bool alie) => alie ? ProximityService.Layer.CombatReservedA : ProximityService.Layer.CombatReservedB;

    private readonly RaycastService raycastService;
    private readonly ProximityService proximityService;

    private int idCounter;
    private readonly Dictionary<int, CombatAgent> agents = new Dictionary<int, CombatAgent>();

    public CombatSystem(RaycastService raycastService, ProximityService proximityService) {
        this.raycastService = raycastService;
        this.proximityService = proximityService;
    }

    public void Update() {
        ResolveDamage();
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
        var proximityLayer = MapFactionToProximityLayer(agent.Alie);
        proximityService.RegisterPoint(position, agentId, proximityLayer);
        return agentId;
    }

    public void UnregisterAgent(int agentId) {
        agents.Remove(agentId, out var agent);
        raycastService.UnregisterMarker(agentId);
        var proximityLayer = MapFactionToProximityLayer(agent.Alie);
        proximityService.RemoveBeacon(agentId, proximityLayer);
    }

    public void UpdateAgentPosition(int agentId, Vector3 position) {
        var agent = agents[agentId];
        agent.Position = position;
        raycastService.UpdateMarker(agentId, position);
        var proximityLayer = MapFactionToProximityLayer(agent.Alie);
        proximityService.UpdatePoint(agentId, position, proximityLayer);
    }

    public bool ApplyProjectileDamage(int agentId, Vector3 position, Vector3 direction, float testDistance, int damage, out Vector3 hitDirection, out ContactSurface surface) {
        hitDirection = Vector3.zero;
        surface = ContactSurface.None;
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
            surface = hitAgent.Config.surface;
        }
        return true;
    }

    public void RecoverFromExplosion(int agentId) {
        if (agents.TryGetValue(agentId, out var agent)) {
            agent.Exploded = false;
        }
    }

    public int ApplyExplosionDamage(int sourceAgentId, Vector3 position, float triggerRadius, int damage, ExplosionData explosionData) {
        var sourceAgent = agents[sourceAgentId];
        var enemyFaction = !sourceAgent.Alie;
        var enemyLayerCode = MapFactionToLayerCode(enemyFaction);
        
        var overlapCount = raycastService.Overlap(position, triggerRadius, enemyLayerCode, out var agentIdResults);
        int affectedCount = 0;
        for (int i = 0; i < overlapCount; i++) {
            var affectedAgent = agents[agentIdResults[i]];
            if (affectedAgent.Id != sourceAgentId && !affectedAgent.Exploded) {
                affectedAgent.Exploded = true;
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
        var enemyFaction = !sourceAgent.Alie;
        var enemyProximityLayer = MapFactionToProximityLayer(enemyFaction);
        
        if (proximityService.QueryNearestBeacon(sourceAgentPosition, out var closestEnemyId, enemyProximityLayer)) {
            var closestEnemy = agents[closestEnemyId];
            if (Vector3.Distance(sourceAgentPosition, closestEnemy.Position) < radius) {
                agentInfo = GetAgentInfo(closestEnemy);
                return true;
            }
        }
        
        agentInfo = default;
        return false;
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

}