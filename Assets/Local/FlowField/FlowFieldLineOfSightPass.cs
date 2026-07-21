using System;
using System.Collections.Generic;

using UnityEngine;

public static class FlowFieldLineOfSightPass {

    public static bool enabled = false;

    public static readonly Vector2Int[] CardinalNeighborsOffsets = new Vector2Int[] {
        new(0, -1),
        new(+1, 0),
        new(0, +1),
        new(-1, 0),
    };

    public static void ComputeLineOfSight(FlowField field, Vector2Int goal, List<Vector2Int> wavefrontOutput) {
        var queue = new Queue<Vector2Int>();
        if (!enabled) {
            wavefrontOutput.Add(goal);
            return;
        }

        var goalIntCell = field[goal.x, goal.y]; // ref was used here
        goalIntCell.SetFlag(CellFlags.HasLineOfSight);
        goalIntCell.integratedCost = 0;
        queue.Enqueue(goal);

        while (queue.Count > 0)
            StepLineOfSight(queue, field, goal, wavefrontOutput);
    }

    internal static void StepLineOfSight(Queue<Vector2Int> queue, FlowField field, Vector2Int goalCell, List<Vector2Int> wavefrontOutput) {
        var current = queue.Dequeue();
        var currentCell = field[current.x, current.y]; // ref was used here

        bool isLosCorner = false;
        foreach (var offset in CardinalNeighborsOffsets) {
            var neighbor = current + offset;

            if (!field.IsInBounds(neighbor.x, neighbor.y))
                continue;

            if (field[neighbor.x, neighbor.y].cost > Cell.DefaultCost) {
                if (IsLosCorner(field, current, neighbor, goalCell)) {
                    CastShadowRay(field, current, goalCell);
                    isLosCorner = true;
                    break;
                }
            }
        }

        if (isLosCorner) {
            currentCell.UnsetFlag(CellFlags.HasLineOfSight);
            wavefrontOutput.Add(current);
            return;
        }

        if (currentCell.HasFlag(CellFlags.WaveFrontBlocked)) {
            currentCell.UnsetFlag(CellFlags.HasLineOfSight);
            wavefrontOutput.Add(current);
            return;
        }

        foreach (var offset in CardinalNeighborsOffsets) {
            var neighbor = current + offset;

            if (!field.IsInBounds(neighbor.x, neighbor.y))
                continue;

            if (field[neighbor.x, neighbor.y].cost > Cell.DefaultCost)
                continue;

            var neighborCell = field[neighbor.x, neighbor.y]; // ref was used here
            if (neighborCell.HasFlag(CellFlags.HasLineOfSight))
                continue;

            neighborCell.integratedCost = currentCell.integratedCost + 1;

            if (neighborCell.HasFlag(CellFlags.WaveFrontBlocked)) {
                wavefrontOutput.Add(neighbor);
                continue;
            }

            neighborCell.SetFlag(CellFlags.HasLineOfSight);
            queue.Enqueue(neighbor);
        }
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
            if (!field.IsInBounds(cx, cy))
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