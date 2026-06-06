using System.Collections.Generic;
using UnityEngine;

namespace FlowFieldPro
{
    /// <summary>
    /// Phase C: Cost integration — Dijkstra wavefront expansion.
    /// </summary>
    public static class CostIntegrationPass
    {
        /// <summary>
        /// Dijkstra-style wavefront expansion using 4 cardinal neighbors.
        /// </summary>
        public static void IntegrateCosts(FlowTile tile, Queue<Vector2Int> wavefront)
        {
            int w = tile.Width;
            int h = tile.Height;

            while (wavefront.Count > 0)
            {
                var current = wavefront.Dequeue();
                ref var currentCell = ref tile.Integration[current.x, current.y];
                ushort currentCost = currentCell.BestCost;

                foreach (var dir in Directions.Cardinal)
                {
                    var offset = Directions.Offset(dir);
                    int nx = current.x + offset.x;
                    int ny = current.y + offset.y;

                    if (nx < 0 || nx >= w || ny < 0 || ny >= h)
                        continue;

                    if (tile.Cost.IsWall(nx, ny))
                        continue;

                    byte cellCost = tile.Cost[nx, ny];
                    int newCost = currentCost + cellCost;

                    if (newCost >= IntegrationField.Unreachable)
                        continue;

                    ref var neighborCell = ref tile.Integration[nx, ny];
                    if ((ushort)newCost < neighborCell.BestCost)
                    {
                        neighborCell.BestCost = (ushort)newCost;
                        wavefront.Enqueue(new Vector2Int(nx, ny));
                    }
                }
            }
        }
    }
}
