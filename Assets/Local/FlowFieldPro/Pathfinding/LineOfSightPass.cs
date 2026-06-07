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

                foreach (var dir in Directions.Cardinal)
                {
                    var offset = Directions.Offset(dir);
                    var neighbor = current + offset;

                    if (neighbor.x < 0 || neighbor.x >= w || neighbor.y < 0 || neighbor.y >= h)
                        continue;

                    int neighborIdx = neighbor.y * w + neighbor.x;
                    ref var neighborInt = ref tile.Integration[neighbor.x, neighbor.y];

                    if (tile.Cost[neighbor.x, neighbor.y] > CostField.DefaultCost)
                    {
                        if (IsLosCorner(tile, current, neighbor))
                            CastShadowRay(tile, current, goalCell);
                        continue;
                    }

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

                    if (visited[neighborIdx])
                        continue;

                    visited[neighborIdx] = true;

                    neighborInt.BestCost = (ushort)(currentCost + 1);
                    neighborInt.Flags |= CellFlags.HasLineOfSight;
                    queue.Enqueue(neighbor);
                }
            }
        }

        private static int GetCostOutOfBoundsAsWalls(FlowTile tile, int x, int y)
        {
            if (!tile.Cost.InBounds(x, y))
                return CostField.Wall;
            return tile.Cost[x, y];
        }

        private static bool IsLosCorner(FlowTile tile, Vector2Int cell, Vector2Int neighbor)
        {
            int dx = neighbor.x - cell.x;
            if (dx != 0)
            {
                bool west = GetCostOutOfBoundsAsWalls(tile, cell.x - 1, cell.y) > 1;
                bool east = GetCostOutOfBoundsAsWalls(tile, cell.x + 1, cell.y) > 1;
                return west != east;
            }
            else
            {
                bool north = GetCostOutOfBoundsAsWalls(tile, cell.x, cell.y - 1) > 1;
                bool south = GetCostOutOfBoundsAsWalls(tile, cell.x, cell.y + 1) > 1;
                return north != south;
            }
        }

        private static int CastShadowRay(FlowTile tile, Vector2Int corner, Vector2Int goal)
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
                return 0;

            int distance = 0;
            while (true)
            {
                if (cx < 0 || cx >= w || cy < 0 || cy >= h)
                    break;

                if (tile.Cost.IsWall(cx, cy))
                    break;

                ref var cell = ref tile.Integration[cx, cy];
                if (cx != x1 && cy != y1) {
                    cell.Flags |= CellFlags.WaveFrontBlocked;
                }

                distance++;
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
            if (distance > 1) {
                ref var cell = ref tile.Integration[x1, y1];
                cell.Flags |= CellFlags.WaveFrontBlocked;
            }
            return distance;
        }
    }
}
