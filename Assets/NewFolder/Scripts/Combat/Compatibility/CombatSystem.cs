using System;
using System.Collections.Generic;

using UnityEngine;

namespace Compatibility {
    public class CombatSystem {

        private static ReservedLayerCode MapFactionToLayerCode(bool alie) => alie ? ReservedLayerCode.First : ReservedLayerCode.Second;
        private static ProximityService.Layer MapFactionToProximityLayer(bool alie) => alie ? ProximityService.Layer.CombatReservedA : ProximityService.Layer.CombatReservedB;

        private readonly RaycastService raycastService;
        private readonly ProximityService proximityService;

        private int idCounter;
        private readonly Dictionary<int, CombatAgent> agents = new Dictionary<int, CombatAgent>();
        private readonly Dictionary<int, int> proximityToAgent = new Dictionary<int, int>();
        private readonly Dictionary<int, int> hitboxToAgent = new Dictionary<int, int>();

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
            var proximityLayer = MapFactionToProximityLayer(agent.Alie);
            agent.ProximityId = proximityService.AddPoint(position, proximityLayer);
            proximityToAgent[agent.ProximityId] = agentId;
            var layerCode = MapFactionToLayerCode(agent.Alie);
            agent.HitboxId = raycastService.RegisterMarker(position, prototype.markerPrefab, layerCode);
            hitboxToAgent[agent.HitboxId] = agentId;
            return agentId;
        }

        public void UnregisterAgent(int agentId) {
            agents.Remove(agentId, out var agent);
            raycastService.UnregisterMarker(agent.HitboxId);
            hitboxToAgent.Remove(agent.HitboxId);
            proximityService.RemovePoint(agent.ProximityId);
            proximityToAgent.Remove(agent.ProximityId);
        }

        public void UpdateAgentPosition(int agentId, Vector3 position) {
            var agent = agents[agentId];
            agent.Position = position;
            raycastService.UpdateMarker(agent.HitboxId, position);
            proximityService.UpdatePoint(agent.ProximityId, position);
        }

        public bool ApplyProjectileDamage(int agentId, Vector3 position, Vector3 direction, float testDistance, int damage, out Vector3 hitDirection, out ContactSurface surface) {
            hitDirection = Vector3.zero;
            surface = ContactSurface.None;
            if (!agents.TryGetValue(agentId, out var agent)) {
                return false;
            }

            var enemyFaction = !agent.Alie;
            var enemyLayerCode = MapFactionToLayerCode(enemyFaction);
            if (!raycastService.Raycast(new Ray(position, direction), testDistance, enemyLayerCode, out var hitboxId, out var hitInfo)) {
                return false;
            }

            var hitAgent = agents[hitboxToAgent[hitboxId]];
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

            var overlapCount = raycastService.Overlap(position, triggerRadius, enemyLayerCode, out var hitboxIdResults);
            int affectedCount = 0;
            for (int i = 0; i < overlapCount; i++) {
                var affectedAgent = agents[hitboxToAgent[hitboxIdResults[i]]];
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

            if (proximityService.QueryNearestPoint(sourceAgentPosition, enemyProximityLayer, out var closestProximtyId)) {
                var closestEnemy = agents[proximityToAgent[closestProximtyId]];
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
}
