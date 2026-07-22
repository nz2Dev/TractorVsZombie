using System;

using UnityEngine;

public static class CornerDetector {

    internal static bool IsLosCorner(FlowField field, Vector2Int cell, Vector2Int goal) {
        foreach (var direction in Directions.Cardinal) {
            var neighbor = cell + Directions.Offset(direction);

            if (!field.IsInBounds(neighbor.x, neighbor.y))
                continue;

            var neighborIsObstacle = field[neighbor.x, neighbor.y].cost > Cell.DefaultCost;
            if (neighborIsObstacle) {
                var obstacle = neighbor;
                if (IsPerpendicularNotBlocked(field, cell, obstacle, goal)) {
                    return true;
                }
            }
        }
        return false;
    }

    internal static bool IsPerpendicularNotBlocked(FlowField field, Vector2Int cell, Vector2Int obstacle, Vector2Int goal) {
        int dx = obstacle.x - cell.x;
        if (dx != 0) {
            int gy = goal.y - cell.y;
            int gx = goal.x - cell.x;

            if (gy == 0 || gx == 0)
                return false;

            if (gx < 0 && dx > 0 || gx > 0 && dx < 0)
                return false;

            var awayCell = new Vector2Int(cell.x, cell.y + Math.Sign(gy));
            if (!field.IsInBounds(awayCell.x, awayCell.y))
                return true;

            var awayIsDefaultCost = field[awayCell.x, awayCell.y].cost == Cell.DefaultCost;
            return awayIsDefaultCost;
        } else {
            int gx = goal.x - cell.x;
            int gy = goal.y - cell.y;

            if (gx == 0 || gy == 0)
                return false;

            int dy = obstacle.y - cell.y;
            if (gy < 0 && dy > 0 || gy > 0 && dy < 0)
                return false;

            var awayCell = new Vector2Int(cell.x + Math.Sign(gx), cell.y);
            if (!field.IsInBounds(awayCell.x, awayCell.y))
                return true;

            var awayIsDefaultCost = field[awayCell.x, awayCell.y].cost == Cell.DefaultCost;
            return awayIsDefaultCost;
        }
    }
}