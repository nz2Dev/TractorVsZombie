using System;
using System.Collections.Generic;

using UnityEngine;

public class PathfindingService {

    private readonly FlowFieldSurface surface;

    private int idCounter;
    private Dictionary<int, FlowField> registry = new();

    public PathfindingService(FlowFieldSurface surface) {
        this.surface = surface;
    }

    public int CreateFlowField(Vector3 goal) {
        var nextFlowFieldId = ++idCounter;
        var goalGridPosition = surface.GetGridPositionClamped(goal);
        
        var flowField = new FlowField(surface.Size, surface.BlockedCells, goalGridPosition);
        flowField.ComputeCosts();
        flowField.ComputeFlow();
        registry[nextFlowFieldId] = flowField;
        return nextFlowFieldId;
    }

    public void UpdateGoal(int fieldId, Vector3 positionWorldSpace) {
        var flowField = registry[fieldId];
        registry[fieldId] = flowField;
        var goalGridPosition = surface.GetGridPositionClamped(positionWorldSpace);
        flowField.SetNextGoal(goalGridPosition);
        flowField.ComputeCosts();
        flowField.ComputeFlow();
    }

    public virtual Vector3 GetFlowVector(int fieldId, Vector3 positionWorldSpace) {
        var flowField = registry[fieldId];
        var gridPosition = surface.GetGridPositionClamped(positionWorldSpace);
        var gridVector = flowField.GetFlowVector(gridPosition.x, gridPosition.y);
        return new Vector3(gridVector.x, 0, gridVector.y).normalized;
    }

    public int RegisterObstacle(Vector3 position, int radius) {
        var obstacleId = surface.AddBlockerShape(position, radius);
        surface.BakeDynamicBlockers();

        foreach (var flowField in registry.Values) {
            flowField.UpdateBlockedCells(surface.BlockedCells);
            flowField.ComputeCosts();
            flowField.ComputeFlow();
        }

        return obstacleId;
    }

    internal void UnregisterObstacle(int obstacleId) {
        surface.RemoveBlockerShape(obstacleId);
        surface.BakeDynamicBlockers();

        foreach (var flowField in registry.Values) {
            flowField.UpdateBlockedCells(surface.BlockedCells);
            flowField.ComputeCosts();
            flowField.ComputeFlow();
        }
    }
}