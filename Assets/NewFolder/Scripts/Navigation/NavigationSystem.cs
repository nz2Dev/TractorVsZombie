using System.Collections.Generic;

using UnityEngine;

public class NavigationSystem {
    
    private PathfindingService pathfindingService;

    public NavigationSystem(PathfindingService pathfindingService) {
        this.pathfindingService = pathfindingService;
    }

    private int idCounter;
    private Dictionary<int, NavigationAgent> registry = new();

    private int destinationIdCounter;
    private Dictionary<MarkerId, int> markerToFlowField = new();

    public void Update() {
        ReadExternalState();
        ProcessLogic();
    }

    public MarkerId CreateMarker(Vector3 position) {
        var markerId = new MarkerId(++destinationIdCounter);
        var flowFieldId = pathfindingService.CreateFlowField(position);
        markerToFlowField[markerId] = flowFieldId;
        return markerId;
    }

    public void UpdateMarkerPosition(MarkerId markerId, Vector3 position) {
        var markerFlowFieldId = markerToFlowField[markerId];
        pathfindingService.UpdateGoal(markerFlowFieldId, position);
    }

    public int AddAgent(Vector3 position, float maxSpeed) {
        var nextId = ++idCounter;
        var agent = new NavigationAgent(nextId, position);
        agent.MaxSpeed = maxSpeed;
        agent.NextPosition = position;
        registry[nextId] = agent;
        return nextId;
    }

    public void RemoveAgent(int id) {
        registry.Remove(id);
    }

    public void SetDestination(int id, MarkerId markerId) {
        var agent = registry[id];
        agent.DestinationMarkerId = markerId;
    }

    public void SetNextSteering(int id, SteeringInput steering) {
        var agent = registry[id];
        agent.NextSteering = steering;
    }

    public void SetNextPosition(int id, Vector3 position) {
        var agent = registry[id];
        agent.NextPosition = position;
    }

    public Vector3 GetComptutedIntent(int id) {
        return registry[id].MovementIntent;
    }

    private void ReadExternalState() {
        foreach (var agent in registry.Values) {
            if (markerToFlowField.TryGetValue(agent.DestinationMarkerId, out var flowFieldId)) {
                agent.FlowDirection = pathfindingService.GetFlowVector(flowFieldId, agent.NextPosition);
            } else {
                agent.FlowDirection = Vector3.zero;
            }
        }
    }

    private void ProcessLogic() {
        foreach (var agent in registry.Values) {
            var steeringInput = agent.NextSteering;
            var direction = Steering.Blend(agent.FlowDirection, agent.NextPosition, steeringInput);
            var speedFactor = Steering.ComputeSpeedFactor(agent.NextPosition, steeringInput);
            var speed = agent.MaxSpeed * speedFactor;
            agent.MovementIntent = direction * speed;
        }
    }

}