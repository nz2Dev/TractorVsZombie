using System.Collections.Generic;
using UnityEngine;

namespace FlowFieldPro
{
    /// <summary>
    /// Phase A: Seed the goal wavefront (goal cells or portal transition cells).
    /// </summary>
    public static class SeedWavefrontPass
    {
        /// <summary>
        /// Seeds the integration field with starting cells at their assigned costs.
        /// Returns the initial wavefront queue for Dijkstra expansion.
        /// </summary>
        public static Queue<Vector2Int> SeedWavefront(FlowTile tile, Vector2Int[] seeds, ushort[] costs)
        {
            var queue = new Queue<Vector2Int>();

            for (int i = 0; i < seeds.Length; i++)
            {
                var cell = seeds[i];
                ushort cost = costs != null ? costs[i] : (ushort)0;

                ref var integrationCell = ref tile.Integration[cell.x, cell.y];
                integrationCell.BestCost = cost;
                integrationCell.Flags |= CellFlags.ActiveWaveFront;
                queue.Enqueue(cell);
            }

            return queue;
        }
    }
}
