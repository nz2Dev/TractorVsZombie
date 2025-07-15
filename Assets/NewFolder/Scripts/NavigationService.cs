using System;
using System.Collections.Generic;

using UnityEngine;

public class NavigationService {

    private FlowFields flowFields;
    private FlowFieldsSpace space;

    public NavigationService() {
    }

    public void SetupFlowField(int sizeBounds, int density, IEnumerable<BoxCollider> obstacles) {
        space = new FlowFieldsSpace(sizeBounds, density);

        flowFields = new FlowFields();
        flowFields.SetGrid(sizeBounds);

        if (obstacles != null) {
            var blockedCells = new HashSet<Vector2Int>();
            foreach (var boxCollider in obstacles) {
                CellRaycaster.ColliderCast(boxCollider, space, blockedCells);
            }
            
            foreach (var blockedLocation in blockedCells) {
                flowFields.SetCellBlocked(blockedLocation.x, blockedLocation.y, true);
            }
        }

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