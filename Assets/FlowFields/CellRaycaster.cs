using System.Collections.Generic;

using UnityEngine;

public static class CellRaycaster {
    
    public static void ColliderCast(Collider collider, FlowFieldsSpace space, ISet<Vector2Int> collector) {
        var center = collider.bounds.center;
        var extens = collider.bounds.extents;
        var doubleUp = new Vector3(0, collider.bounds.size.y * 2, 0);

        Vector3 bottomLeft = center + new Vector3(-extens.x, 0, -extens.z);
        Vector3 topRight = center + new Vector3(+extens.x, 0, +extens.z);

        var gridStart = space.ConvertToGrid(bottomLeft);
        var gridEnd = space.ConvertToGrid(topRight);
        var rowsSpan = gridEnd.x - gridStart.x;
        var columnSpan = gridEnd.y - gridStart.y;
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
    }

}