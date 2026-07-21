using System;
using System.Collections.Generic;

using UnityEngine;

public static class FlowFieldLineOfSightPass {

    public static void ComputeLineOfSight(FlowField flowField, Vector2Int goal, List<Vector2Int> wavefrontOutput) {
        wavefrontOutput.Clear();
        wavefrontOutput.Add(goal);
    }

    internal static void CastShadowRay(FlowField field, Vector2Int corner, Vector2Int goal) {
        int w = field.Size;
        int h = field.Size;

        int x0 = goal.x;
        int y0 = goal.y;
        int x1 = corner.x;
        int y1 = corner.y;

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        int cx = x1;
        int cy = y1;

        if (cx == x0 && cy == y0)
            return;

        while (true) {
            if (cx < 0 || cx >= w || cy < 0 || cy >= h)
                break;

            if (field[cx, cy].IsBlocked())
                break;

            // ref var cell = ref field[cx, cy]; don't forget to use ref when migrate to structs
            field[cx, cy].SetFlag(CellFlags.WaveFrontBlocked);

            int e2 = 2 * err;
            if (e2 > -dy) {
                err -= dy;
                cx += sx;
            }
            if (e2 < dx) {
                err += dx;
                cy += sy;
            }
        }
    }

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
        }
        else {
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