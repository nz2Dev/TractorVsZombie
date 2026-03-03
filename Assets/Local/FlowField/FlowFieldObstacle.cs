using System.Collections.Generic;

using UnityEngine;

public class FlowFieldObstacle {
    
    private Collider collider;
    
    private Vector3 point;
    private int radius;

    public FlowFieldObstacle(Collider collider) {
        this.collider = collider;
    }

    public FlowFieldObstacle(Vector3 point, int radius) {
        this.point = point;
        this.radius = radius;
    }

    public Vector2Int[] CalculateCells(FlowFieldSpace space) {
        if (collider != null) {
            return CalculateColliderCells(space, collider);
        } else {
            return CalculatePointRadiusCells(space, point, radius);
        }
    }

    private Vector2Int[] CalculateColliderCells(FlowFieldSpace space, Collider collider) {
        var center = collider.bounds.center;
        var extens = collider.bounds.extents;
        var doubleUp = new Vector3(0, collider.bounds.size.y * 2, 0);

        Vector3 bottomLeft = center + new Vector3(-extens.x, 0, -extens.z);
        Vector3 topRight = center + new Vector3(+extens.x, 0, +extens.z);

        var gridStart = space.ConvertToGrid(bottomLeft);
        var gridEnd = space.ConvertToGrid(topRight);
        var rowsSpan = gridEnd.x - gridStart.x;
        var columnSpan = gridEnd.y - gridStart.y;

        var collector = new List<Vector2Int>();
        for (int row = 0; row <= rowsSpan; row++) {
            for (int column = 0; column <= columnSpan; column++) {
                var gridLocation = gridStart + new Vector2Int(row, column);
                var gridWorld = space.ConvertToWorld(gridLocation, atCenter: true);

                var gridRay = new Ray(gridWorld + doubleUp, Vector3.down);
                var raycasted = collider.Raycast(gridRay, out var _, maxDistance: float.MaxValue);
                
                if (raycasted) {
                    collector.Add(gridLocation);
                }
            }
        }

        return collector.ToArray();
    }

    private Vector2Int[] CalculatePointRadiusCells(FlowFieldSpace space, Vector3 point, int radius) {
        var obstacleCells = new List<Vector2Int>();
        var centerGrid = space.ConvertToGridClampled(point);
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                var offset = new Vector2Int(x, y);
                if (offset.sqrMagnitude <= radius * radius) {
                    obstacleCells.Add(centerGrid + offset);
                }
            }
        } 
        return obstacleCells.ToArray();
    }
    
}