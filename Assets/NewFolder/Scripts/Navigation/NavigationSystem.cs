using System.Collections.Generic;

using UnityEngine;

/*
    Produce step-based velocities to get agent's position to its goal
    It accounts for avoidance steering of obstacles and other agents
*/
public class NavigationSystem {
    
    private PathfindingService pathfindingService;
    private LocalAvoidanceService avoidanceService;

    public NavigationSystem(LocalAvoidanceService avoidanceService, PathfindingService pathfindingService) {
        this.avoidanceService = avoidanceService;
        this.pathfindingService = pathfindingService;
    }

    private int idCounter;
    private int formationIdCounter;
    private Dictionary<int, NavigationAgent> registry = new();
    private Dictionary<int, NavigationFormation> formations = new();

    public void Update() {
        ReadExternalState();
        ComputeFormationAverages();
        ProcessLogic();
        WriteExternalState();
    }

    public void SetGoal(Vector3 goal) {
        pathfindingService.SetGoal(goal);
    }

    public int AddAgent(Vector3 position, float maxSpeed, AgentAvoidanceConfig config) {
        var nextId = ++idCounter;
        var agent = new NavigationAgent(nextId, position);
        agent.MaxSpeed = maxSpeed;
        agent.AvoidanceId = avoidanceService.AddAgent(position, config);
        agent.NextPosition = position;
        registry[nextId] = agent;
        return nextId;
    }

    public void RemoveAgent(int id) {
        var agent = registry[id];
        avoidanceService.RemoveAgent(agent.AvoidanceId);
        if (agent.FormationId.HasValue)
            RemoveAgentFromFormation(id);
        registry.Remove(id);
    }

    public void SetNextPosition(int id, Vector3 position) {
        if (registry.TryGetValue(id, out var agent)) {
            agent.NextPosition = position;
        }
    }

    public Vector3 GetComputedVelocity(int id) {
        if (registry.TryGetValue(id, out var agent)) {
            return agent.ComputedVelocity;
        }
        return Vector3.zero;
    }

    public int CreateFormation() {
        var nextId = ++formationIdCounter;
        formations[nextId] = new NavigationFormation(nextId);
        return nextId;
    }

    public void DisbandFormation(int formationId) {
        var formation = formations[formationId];
        foreach (var agentId in formation.AgentIds) {
            registry[agentId].FormationId = null;
        }
        formations.Remove(formationId);
    }

    public void AssignAgentToFormation(int agentId, int formationId) {
        var agent = registry[agentId];
        var formation = formations[formationId];

        if (agent.FormationId.HasValue && agent.FormationId.Value != formationId) {
            RemoveAgentFromFormation(agentId);
        }

        if (!formation.AgentIds.Contains(agentId)) {
            formation.AgentIds.Add(agentId);
        }
        agent.FormationId = formationId;
    }

    public void RemoveAgentFromFormation(int agentId) {
        var agent = registry[agentId];
        if (!agent.FormationId.HasValue)
            return;

        formations[agent.FormationId.Value].AgentIds.Remove(agentId);
        agent.FormationId = null;
    }

    private void ReadExternalState() {
        foreach (var agent in registry.Values) {
            agent.RvoVelocity = avoidanceService.GetVelocity(agent.AvoidanceId);
            agent.FlowDirection = pathfindingService.GetFlowVector(agent.NextPosition);
        }
    }

    private void ProcessLogic() {
        foreach (var agent in registry.Values) {
            var direction = agent.FlowDirection;
            var speed = agent.MaxSpeed;

            if (agent.FormationId.HasValue) {
                var formation = formations[agent.FormationId.Value];
                const float cohesionWeight = 0.3f;
                direction = Vector3.Lerp(direction, formation.AverageDirection, cohesionWeight).normalized;

                const float speedAdjustFactor = 0.2f;
                float relativePosition = Vector3.Dot(agent.NextPosition - formation.CenterPosition, formation.AverageDirection);
                float speedFactor = relativePosition > 0 ? Mathf.Clamp01(1f - relativePosition * speedAdjustFactor) : 1f;
                speed = agent.MaxSpeed * speedFactor;
            }

            agent.MovementIntent = direction * speed;
            agent.ComputedVelocity = agent.RvoVelocity;
        }
    }

    private void ComputeFormationAverages() {
        foreach (var formation in formations.Values) {
            if (formation.AgentIds.Count == 0)
                continue;

            var sumDirection = Vector3.zero;
            var sumPosition = Vector3.zero;
            int count = 0;

            foreach (var agentId in formation.AgentIds) {
                var agent = registry[agentId];
                sumDirection += agent.RvoVelocity;
                sumPosition += agent.NextPosition;
                count++;
            }

            if (count > 0) {
                formation.AverageDirection = (sumDirection / count).normalized;
                formation.CenterPosition = sumPosition / count;
            }
        }
    }

    private void WriteExternalState() {
        foreach (var agent in registry.Values) {
            avoidanceService.SetAgentPosition(agent.AvoidanceId, agent.NextPosition);
            avoidanceService.SetPreferedVelocity(agent.AvoidanceId, agent.MovementIntent);
        }
    }
}