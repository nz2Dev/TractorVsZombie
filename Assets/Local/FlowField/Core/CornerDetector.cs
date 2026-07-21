using System;

using UnityEngine;

public static class CornerDetector {
    internal static bool IsLosCorner(FlowField field, Vector2Int cell, Vector2Int neighbor, Vector2Int goal) {
        int dx = neighbor.x - cell.x;
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

            var awayIsBlocked = field[awayCell.x, awayCell.y].cost > 1;
            return !awayIsBlocked;
        } else {
            int gx = goal.x - cell.x;
            int gy = goal.y - cell.y;

            if (gx == 0 || gy == 0)
                return false;

            int dy = neighbor.y - cell.y;
            if (gy < 0 && dy > 0 || gy > 0 && dy < 0)
                return false;

            var awayCell = new Vector2Int(cell.x + Math.Sign(gx), cell.y);
            if (!field.IsInBounds(awayCell.x, awayCell.y))
                return true;

            var awayIsBlocked = field[awayCell.x, awayCell.y].cost > 1;
            return !awayIsBlocked;
        }
    }
}