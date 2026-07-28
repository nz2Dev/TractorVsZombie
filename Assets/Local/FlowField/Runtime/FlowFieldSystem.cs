using System;
using System.Collections.Generic;

using UnityEngine;

public class FlowFieldSystem {
    
    public static FlowFieldSystem Instance;

    // todo add explanation why setting space from FlowFieldSpaceSource has no effect if system is used during Start() and the source didn't get created yet
    private FlowFieldSpace space = new FlowFieldSpace(100, 1);
    private List<FlowFieldObstacle> obstacles = new();
    private List<FlowFieldHandle> flowFieldsHandles = new();

    public IReadOnlyList<FlowFieldHandle> Handles => flowFieldsHandles;

    private HashSet<Vector2Int> blockedCells = new();
    private bool obstaclesIsDirty = false;
    private bool spaceIsDirty = false;

    internal void Update() {
        if (spaceIsDirty) {
            RecreateFields();
            spaceIsDirty = false;
        }

        if (obstaclesIsDirty) {
            RebuildObstacles();
            obstaclesIsDirty = false;
        }

        foreach (var flowFieldHandle in flowFieldsHandles) {
            if (flowFieldHandle.computeIsDirty) {
                var field = flowFieldHandle.flowField;
                var goal = space.ConvertToGridClampled(flowFieldHandle.goal);
                FlowFieldIntegrator.Integrate(field, goal, lineOfSightPass: true);
                flowFieldHandle.computeIsDirty = false;
            }
        }
    }

    public void SetSpace(FlowFieldSpace space) {
        // todo fix space overriding creates state desync, when fields use one space and system the newly assigned
        this.space = space;
        spaceIsDirty = true;
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
        var flowField = new FlowField(space.Size, blockedCells);
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
        var gridCell = handle.flowField[gridPosition.x, gridPosition.y];
        if (gridCell.HasFlag(CellFlags.HasLineOfSight)) {
            return (handle.goal - position).normalized;
        } else {
            var flowDirection = gridCell.flowVector;
            return new Vector3(flowDirection.x, 0, flowDirection.y).normalized;
        }
    }

    private void RecreateFields() {
        foreach (var flowFieldHandle in flowFieldsHandles) {
            flowFieldHandle.flowField = new FlowField(space.Size, blockedCells);
            flowFieldHandle.computeIsDirty = true;
        }
    }

    private void RebuildObstacles() {
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