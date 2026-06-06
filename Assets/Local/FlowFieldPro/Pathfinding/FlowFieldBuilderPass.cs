using UnityEngine;

namespace FlowFieldPro
{
    /// <summary>
    /// Phase D: Build flow field — pick steepest descent direction per cell.
    /// </summary>
    public static class FlowFieldBuilderPass
    {
        /// <summary>
        /// For each reachable cell, determines the best flow direction by examining
        /// all 8 neighbors and picking the one with the lowest integrated cost.
        /// </summary>
        public static void BuildFlowField(FlowTile tile)
        {
            int w = tile.Width;
            int h = tile.Height;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    ref var integrationCell = ref tile.Integration[x, y];

                    if (tile.Cost.IsWall(x, y) || integrationCell.BestCost == IntegrationField.Unreachable)
                    {
                        tile.Flow[x, y] = FlowCell.FromDirection(Direction.None);
                        continue;
                    }

                    if ((integrationCell.Flags & CellFlags.HasLineOfSight) != 0)
                    {
                        tile.Flow[x, y] = FlowCell.FromLineOfSight();
                        continue;
                    }

                    ushort bestCost = integrationCell.BestCost;
                    Direction bestDir = Direction.None;

                    foreach (var dir in Directions.All)
                    {
                        var offset = Directions.Offset(dir);
                        int nx = x + offset.x;
                        int ny = y + offset.y;

                        if (nx < 0 || nx >= w || ny < 0 || ny >= h)
                            continue;

                        if (tile.Cost.IsWall(nx, ny))
                            continue;

                        ushort neighborCost = tile.Integration[nx, ny].BestCost;
                        if (neighborCost < bestCost)
                        {
                            bestCost = neighborCost;
                            bestDir = dir;
                        }
                    }

                    tile.Flow[x, y] = FlowCell.FromDirection(bestDir);
                }
            }
        }
    }
}
