using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFieldPro
{
    /// <summary>
    /// Phase B: LOS pass — mark cells with unobstructed line-of-sight to the goal.
    /// </summary>
    public static class LineOfSightPass
    {
        /// <summary>
        /// Floods outward from the goal cell, marking cells that have unobstructed
        /// line-of-sight as HasLineOfSight.
        /// </summary>
        public static void ComputeLineOfSight(FlowTile tile, Vector2Int goalCell, Queue<Vector2Int> costWavefront)
        {
            int w = tile.Width;
            int h = tile.Height;

            var visited = new bool[w * h];
            var queue = new Queue<Vector2Int>();

            ref var goalIntCell = ref tile.Integration[goalCell.x, goalCell.y];
            goalIntCell.Flags |= CellFlags.HasLineOfSight;
            visited[goalCell.y * w + goalCell.x] = true;
            queue.Enqueue(goalCell);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                ushort currentCost = tile.Integration[current.x, current.y].BestCost;

                bool isLosCorner = false;
                foreach (var dir in Directions.Cardinal)
                {
                    var offset = Directions.Offset(dir);
                    var neighbor = current + offset;

                    if (neighbor.x < 0 || neighbor.x >= w || neighbor.y < 0 || neighbor.y >= h)
                        continue;

                    if (tile.Cost[neighbor.x, neighbor.y] > CostField.DefaultCost)
                    {
                        if (IsLosCorner(tile.Cost, current, neighbor, goalCell))
                        {
                            CastShadowRay(tile, current, goalCell);
                            ref var currentInt = ref tile.Integration[current.x, current.y];
                            currentInt.Flags &= ~CellFlags.HasLineOfSight;
                            isLosCorner = true;
                            break;
                        }
                    }
                }

                if (isLosCorner)
                    continue;

                foreach (var dir in Directions.Cardinal)
                {
                    var offset = Directions.Offset(dir);
                    var neighbor = current + offset;

                    if (neighbor.x < 0 || neighbor.x >= w || neighbor.y < 0 || neighbor.y >= h)
                        continue;

                    if (tile.Cost[neighbor.x, neighbor.y] > CostField.DefaultCost)
                        continue;
                    
                    ref var neighborInt = ref tile.Integration[neighbor.x, neighbor.y];
                    if ((neighborInt.Flags & CellFlags.WaveFrontBlocked) != 0)
                    {
                        ushort potentialCost = (ushort)(currentCost + 1);
                        if (potentialCost < neighborInt.BestCost)
                        {
                            neighborInt.BestCost = potentialCost;
                            costWavefront.Enqueue(neighbor);
                        }
                        continue;
                    }

                    int neighborIdx = neighbor.y * w + neighbor.x;
                    if (visited[neighborIdx])
                        continue;

                    visited[neighborIdx] = true;

                    neighborInt.BestCost = (ushort)(currentCost + 1);
                    neighborInt.Flags |= CellFlags.HasLineOfSight;
                    queue.Enqueue(neighbor);
                }
            }
        }

        internal static bool IsLosCorner(CostField tile, Vector2Int cell, Vector2Int neighbor, Vector2Int goal)
        {
            int dx = neighbor.x - cell.x;
            if (dx != 0)
            {
                int gy = goal.y - cell.y;
                int gx = goal.x - cell.x;
                
                if (gy == 0 || gx == 0)
                    return false;

                if (gx < 0 && dx > 0 || gx > 0 && dx < 0)
                    return false;

                var awayCell = new Vector2Int(cell.x, cell.y + Math.Sign(gy));
                if (!tile.InBounds(awayCell.x, awayCell.y))
                    return true;

                var awayIsBlocked = tile[awayCell.x, awayCell.y] > 1;
                return !awayIsBlocked;
            }
            else
            {
                int gx = goal.x - cell.x;
                int gy = goal.y - cell.y;
                
                if (gx == 0 || gy == 0)
                    return false;

                int dy = neighbor.y - cell.y;
                if (gy < 0 && dy > 0 || gy > 0 && dy < 0)
                    return false;

                var awayCell = new Vector2Int(cell.x + Math.Sign(gx), cell.y);
                if (!tile.InBounds(awayCell.x, awayCell.y))
                    return true;

                var awayIsBlocked = tile[awayCell.x, awayCell.y] > 1;
                return !awayIsBlocked;
            }
        }

        internal static void CastShadowRay(FlowTile tile, Vector2Int corner, Vector2Int goal)
        {
            int w = tile.Width;
            int h = tile.Height;

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

            while (true)
            {
                if (cx < 0 || cx >= w || cy < 0 || cy >= h)
                    break;

                if (tile.Cost.IsWall(cx, cy))
                    break;

                ref var cell = ref tile.Integration[cx, cy];
                cell.Flags |= CellFlags.WaveFrontBlocked;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    cx += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    cy += sy;
                }
            }
        }
    }
}
