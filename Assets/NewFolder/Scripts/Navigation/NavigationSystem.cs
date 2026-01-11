using System.Collections.Generic;

using UnityEngine;

/*
    Produce step-based velocities to get agent's position to its goal
    It accounts for avoidance steering of obstacles and other agents
*/
public class NavigationSystem {
    
    private NavigationService navigationService;
    private LocalAvoidanceService avoidanceService;

    public NavigationSystem(LocalAvoidanceService avoidanceService, NavigationService navigationService) {
        this.avoidanceService = avoidanceService;
        this.navigationService = navigationService;
    }

    private int idCounter;
    private Dictionary<int, NavigationAgent> registry = new();

    public void Update() {
        ReadExternalState();
        ProcessLogic();
        WriteExternalState();
    }

    private void ReadExternalState() {
        foreach (var agent in registry.Values) {
            agent.RvoVelocity = avoidanceService.GetVelocity(agent.AvoidanceId);
            agent.FlowDirection = navigationService.GetFlowVector(agent.NextPosition);
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

    public int AddAgent(Vector3 position, float maxSpeed) {
        var nextId = ++idCounter;
        var agent = new NavigationAgent(nextId, position);
        agent.MaxSpeed = maxSpeed;
        agent.AvoidanceId = avoidanceService.AddAgent(position);
        registry[nextId] = agent;
        return nextId;
    }

    public void SetNextPosition(int id, Vector3 position) {
        if (registry.TryGetValue(id, out var agent)) {
            agent.NextPosition = position;
        }
    }

    public void SetGoal(int id, Vector3 goal) {
         if (registry.TryGetValue(id, out var agent)) {
            agent.Goal = goal;
        }
    }

    public Vector3 GetComputedMovement(int id) {
        if (registry.TryGetValue(id, out var agent)) {
            return agent.ComputedVelocity;
        }
        return Vector3.zero;
    }
}