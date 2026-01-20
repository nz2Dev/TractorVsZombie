using System;
using System.Collections.Generic;

using UnityEngine;

public class PathfindingService {

    private readonly FlowFieldSurface surface;
    private FlowField flowField;
    private Vector3 worldSpaceGoal;

    public PathfindingService(FlowFieldSurface surface) {
        this.surface = surface;
    }

    public virtual Vector3 GetFlowVector(Vector3 worldSpacePosition) {
        var gridPosition = surface.GetGridPositionClamped(worldSpacePosition);
        var gridVector = flowField.GetFlowVector(gridPosition.x, gridPosition.y);
        return new Vector3(gridVector.x, 0, gridVector.y).normalized;
    }

    public virtual void SetGoal(Vector3 worldSpacePosition) {
        worldSpaceGoal = worldSpacePosition;
        var goalGridPosition = surface.GetGridPositionClamped(worldSpacePosition);
        flowField = new FlowField(surface.Size, surface.BlockedCells, goalGridPosition);
        flowField.ComputeCosts();
        flowField.ComputeFlow();
    }

    public Vector3 GetGoal() {
        return worldSpaceGoal;
    }

}