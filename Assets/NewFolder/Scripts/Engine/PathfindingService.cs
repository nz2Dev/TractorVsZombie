using System;
using System.Collections.Generic;

using UnityEngine;

public class PathfindingService {

    private readonly FlowFieldSystem system;

    private int idCounter;
    private Dictionary<int, FlowFieldHandle> registry = new();

    private int obstacleIdCounter;
    private Dictionary<int, FlowFieldObstacle> obstacleRegistry = new();

    public PathfindingService(FlowFieldSystem system) {
        this.system = system;
    }

    public int CreateFlowField(Vector3 goal) {
        var nextFlowFieldId = ++idCounter;
        var handle = system.CreateField(goal);
        registry[nextFlowFieldId] = handle;
        return nextFlowFieldId;
    }

    public void UpdateGoal(int fieldId, Vector3 positionWorldSpace) {
        var flowFieldHandle = registry[fieldId];
        system.SetFieldGoal(flowFieldHandle, positionWorldSpace);
    }

    public virtual Vector3 GetFlowVector(int fieldId, Vector3 positionWorldSpace) {
        var flowFieldHandle = registry[fieldId];
        return system.GetFlowVector(flowFieldHandle, positionWorldSpace);
    }

    public int RegisterObstacle(Vector3 position, int radius) {
        var obstacle = system.AddObstacle(position, radius);
        var nextObstacleId = ++obstacleIdCounter;
        obstacleRegistry[nextObstacleId] = obstacle;
        return nextObstacleId;
    }

    public void UnregisterObstacle(int obstacleId) {
        obstacleRegistry.Remove(obstacleId, out var obstacle);
        system.RemoveObstacle(obstacle);
    }

}