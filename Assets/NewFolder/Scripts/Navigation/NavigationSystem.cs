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
    private Dictionary<int, NavigationAgent> registry = new();

    public void Update() {
        ReadExternalState();
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
        if (registry.TryGetValue(id, out var agent)) {
            avoidanceService.RemoveAgent(agent.AvoidanceId);
            registry.Remove(id);
        }
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

    private void ReadExternalState() {
        foreach (var agent in registry.Values) {
            agent.RvoVelocity = avoidanceService.GetVelocity(agent.AvoidanceId);
            agent.FlowDirection = pathfindingService.GetFlowVector(agent.NextPosition);
        }
    }

    private void ProcessLogic() {
        foreach (var agent in registry.Values) {
            agent.MovementIntent = agent.FlowDirection * agent.MaxSpeed;
            agent.ComputedVelocity = agent.RvoVelocity;
        }
    }

    private void WriteExternalState() {
        foreach (var agent in registry.Values) {
            avoidanceService.SetAgentPosition(agent.AvoidanceId, agent.NextPosition);
            avoidanceService.SetPreferedVelocity(agent.AvoidanceId, agent.MovementIntent);
        }
    }
}