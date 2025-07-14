using System;

using UnityEngine;

public class NavigationService {

    private FlowFields flowFields;
    private FlowFieldsSpace space;

    public NavigationService() {
    }

    public void SetupFlowField(int sizeBounds, int density, object obstacles) {
        flowFields = new FlowFields();
        flowFields.SetGrid(sizeBounds);
        space = new FlowFieldsSpace(sizeBounds, density);
    }

    public Vector3 GetFlowVector(Vector3 worldSpacePosition) {
        var gridLocation = space.ConvertToGrid(worldSpacePosition);
        var flowVector2Int = flowFields.GetFlowVector(gridLocation.x, gridLocation.y);
        return new Vector3(flowVector2Int.x, 0, flowVector2Int.y).normalized;
    }

    public void SetGoal(Vector3 worldSpacePosition) {
        var goalGridLocation = space.ConvertToGrid(worldSpacePosition);
        flowFields.ComputeCosts(goalGridLocation);
        flowFields.ComputeFlow();
    }
}