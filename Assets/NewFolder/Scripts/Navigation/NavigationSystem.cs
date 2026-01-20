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

    public int AddAgent(Vector3 position, int flowFieldId, float maxSpeed, AgentAvoidanceConfig config) {
        var nextId = ++idCounter;
        var agent = new NavigationAgent(nextId, position);
        agent.MaxSpeed = maxSpeed;
        agent.AvoidanceId = avoidanceService.AddAgent(position, config);
        agent.FlowFieldId = flowFieldId;
        agent.NextPosition = position;
        registry[nextId] = agent;
        return nextId;
    }

    public void RemoveAgent(int id) {
        var agent = registry[id];
        avoidanceService.RemoveAgent(agent.AvoidanceId);
        registry.Remove(id);
    }

    public void SetNextSteering(int id, SteeringInput steering) {
        var agent = registry[id];
        agent.NextSteering = steering;
    }

    public void SetNextPosition(int id, Vector3 position) {
        var agent = registry[id];
        agent.NextPosition = position;
    }

    public Vector3 GetComputedVelocity(int id) {
        return registry[id].ComputedVelocity;
    }

    private void ReadExternalState() {
        foreach (var agent in registry.Values) {
            agent.RvoVelocity = avoidanceService.GetVelocity(agent.AvoidanceId);
            agent.FlowDirection = pathfindingService.GetFlowVector(agent.FlowFieldId, agent.NextPosition);
        }
    }

    private void ProcessLogic() {
        foreach (var agent in registry.Values) {
            var steeringInput = agent.NextSteering;
            var direction = Steering.Blend(agent.FlowDirection, agent.NextPosition, steeringInput);
            var speedFactor = Steering.ComputeSpeedFactor(agent.NextPosition, steeringInput);
            var speed = agent.MaxSpeed * speedFactor;
            agent.MovementIntent = direction * speed;
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