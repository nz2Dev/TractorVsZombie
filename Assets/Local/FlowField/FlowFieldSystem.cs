using System;
using System.Collections.Generic;

using UnityEngine;

public class FlowFieldSystem {
    
    public static FlowFieldSystem Instance;

    private FlowFieldSpace space = new FlowFieldSpace(50, 1);
    private List<FlowFieldObstacle> obstacles = new();
    private List<FlowFieldHandle> flowFieldsHandles = new();

    private HashSet<Vector2Int> blockedCells = new();
    private bool obstaclesIsDirty = false;

    internal void Update() {
        if (obstaclesIsDirty) {
            RebuildFields();
            obstaclesIsDirty = false;
        }

        foreach (var flowFieldHandle in flowFieldsHandles) {
            if (flowFieldHandle.computeIsDirty) {
                var fields = flowFieldHandle.flowField;
                fields.SetNextGoal(space.ConvertToGridClampled(flowFieldHandle.goal));
                fields.ComputeCosts();
                fields.ComputeFlow();
                flowFieldHandle.computeIsDirty = false;
            }
        }
    }

    public void SetSpace(FlowFieldSpace space) {
        this.space = space;
    }

    public FlowFieldObstacle AddObstacle(Collider collider) {
        var obstacle = new FlowFieldObstacle(collider);
        obstacles.Add(obstacle);
        obstaclesIsDirty = true;
        return obstacle;
    }

    public FlowFieldObstacle AddObstacle(Vector3 position, int radius) {
        var obstacle = new FlowFieldObstacle(position, radius);
        obstacles.Add(obstacle);
        obstaclesIsDirty = true;
        return obstacle;
    }

    public void RemoveObstacle(FlowFieldObstacle data) {
        obstacles.Remove(data);
        obstaclesIsDirty = true;
    }

    public FlowFieldHandle CreateField(Vector3 initialGoal) {
        var flowField = new FlowField(space.Size, blockedCells, space.ConvertToGridClampled(initialGoal));
        var handle = new FlowFieldHandle { flowField = flowField, goal = initialGoal, computeIsDirty = true };
        flowFieldsHandles.Add(handle);
        return handle;
    }

    public void RemoveField(FlowFieldHandle handle) {
        flowFieldsHandles.Remove(handle);
    }

    public void SetFieldGoal(FlowFieldHandle flowFieldHandle, Vector3 positionWorldSpace) {
        flowFieldHandle.goal = positionWorldSpace;
        flowFieldHandle.computeIsDirty = true;
    }

    public Vector3 GetFlowVector(FlowFieldHandle handle, Vector3 position) {
        var gridPosition = space.ConvertToGridClampled(position);
        var flowDirection = handle.flowField.GetFlowVector(gridPosition.x, gridPosition.y);
        return new Vector3(flowDirection.x, 0, flowDirection.y).normalized;
    }

    private void RebuildFields() {
        blockedCells.Clear();
        foreach (var obstacleData in obstacles) {
            foreach (var cell in obstacleData.CalculateCells(space)) {
                blockedCells.Add(cell);
            }
        }

        foreach (var flowFieldHandle in flowFieldsHandles) {
            flowFieldHandle.flowField.UpdateBlockedCells(blockedCells);
            flowFieldHandle.computeIsDirty = true;
        }
    }

}