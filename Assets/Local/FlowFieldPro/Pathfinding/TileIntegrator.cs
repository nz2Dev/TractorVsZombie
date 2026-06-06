using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFieldPro
{
    /// <summary>
    /// Constructs flow tiles for sectors along a corridor.
    ///
    /// Implements the 4-phase tile construction pipeline from the paper:
    ///   Phase A: Seed the goal wavefront (goal cells or portal transition cells)
    ///   Phase B: LOS pass — mark cells with unobstructed line-of-sight to the goal
    ///   Phase C: Cost integration — Dijkstra wavefront expansion
    ///   Phase D: Build flow field — pick steepest descent direction per cell
    ///
    /// After processing, every reachable cell in every corridor sector has a flow
    /// direction pointing toward the goal. Agents read these directions at runtime.
    /// </summary>
    public static class TileIntegrator
    {
        /// <summary>
        /// Processes a complete path request: builds flow tiles for every sector
        /// in the corridor, starting from the goal sector and propagating costs
        /// outward through portal transitions.
        /// </summary>
        public static void IntegrateRequest(PathRequest request, SectorGrid grid)
        {
            var goalSector = grid.WorldToSector(request.Goal);

            // Create FlowTiles for all corridor sectors
            foreach (var sector in request.CorridorSectors)
            {
                var costField = grid.GetCostField(sector);
                request.Tiles[sector] = new FlowTile(costField);
            }

            // Build the goal sector first (it has the actual goal cell as seed)
            if (request.Tiles.TryGetValue(goalSector, out var goalTile))
            {
                var goalLocal = grid.WorldToLocal(request.Goal);
                IntegrateTile(goalTile, new[] { goalLocal }, null, grid, goalSector);
            }

            // Build remaining corridor sectors in wavefront order (BFS from goal sector)
            // Each non-goal sector is seeded by portal cells carrying integrated costs
            // from the adjacent downstream sector.
            var processed = new HashSet<Vector2Int> { goalSector };
            var sectorQueue = new Queue<Vector2Int>();
            sectorQueue.Enqueue(goalSector);

            while (sectorQueue.Count > 0)
            {
                var currentSector = sectorQueue.Dequeue();
                if (!request.Tiles.TryGetValue(currentSector, out var currentTile))
                    continue;

                // Find portals that connect current sector to unprocessed corridor sectors
                foreach (var portal in FindCorridorPortals(request, currentSector))
                {
                    var neighborSector = portal.GetOtherSector(currentSector);
                    if (processed.Contains(neighborSector))
                        continue;
                    if (!request.Tiles.TryGetValue(neighborSector, out var neighborTile))
                        continue;

                    // Collect seed cells for the neighbor sector from the portal,
                    // carrying integrated costs from the current (downstream) tile
                    var seedCells = new List<Vector2Int>();
                    var seedCosts = new List<ushort>();

                    var currentSideCells = portal.GetCellsForSector(currentSector);
                    var neighborSideCells = portal.GetCellsForSector(neighborSector);

                    for (int i = 0; i < currentSideCells.Length; i++)
                    {
                        var currentLocal = grid.WorldToLocal(currentSideCells[i]);
                        ushort carriedCost = currentTile.Integration[currentLocal.x, currentLocal.y].BestCost;
                        if (carriedCost == IntegrationField.Unreachable)
                            continue;

                        var neighborLocal = grid.WorldToLocal(neighborSideCells[i]);
                        seedCells.Add(neighborLocal);
                        // Add the border crossing cost (cost of the neighbor cell itself)
                        byte cellCost = neighborTile.Cost[neighborLocal.x, neighborLocal.y];
                        seedCosts.Add((ushort)Math.Min(carriedCost + cellCost, ushort.MaxValue - 1));
                    }

                    if (seedCells.Count > 0)
                    {
                        IntegrateTile(neighborTile, seedCells.ToArray(), seedCosts.ToArray(), grid, neighborSector);
                    }

                    processed.Add(neighborSector);
                    sectorQueue.Enqueue(neighborSector);
                }
            }
        }
        /// <summary>
        /// Integrates a single tile through all 4 phases.
        /// </summary>
        /// <param name="tile">The flow tile to process.</param>
        /// <param name="seedCells">Local coordinates of seed cells (goal or portal cells).</param>
        /// <param name="seedCosts">
        /// Pre-assigned costs for each seed cell. Null if seeds should start at cost 0
        /// (i.e., this is the goal sector).
        /// </param>
        /// <param name="grid">The sector grid (used for coordinate conversion).</param>
        /// <param name="sectorCoord">Which sector this tile belongs to.</param>
        public static void IntegrateTile(
            FlowTile tile,
            Vector2Int[] seedCells,
            ushort[] seedCosts,
            SectorGrid grid,
            Vector2Int sectorCoord)
        {
            tile.ResetComputed();

            // Phase A: Seed the wavefront
            var wavefront = SeedWavefront(tile, seedCells, seedCosts);

            // Phase B: LOS pass (only for goal-sector tiles where seeds start at cost 0)
            bool isGoalTile = seedCosts == null;
            if (isGoalTile && seedCells.Length == 1)
            {
                ComputeLineOfSight(tile, seedCells[0], wavefront);
            }

            // Phase C: Cost integration (Dijkstra expansion)
            IntegrateCosts(tile, wavefront);

            // Phase D: Build flow directions
            BuildFlowField(tile);
        }

        // -----------------------------------------------------------------------
        // Phase A: Seed Goal Wavefront
        // -----------------------------------------------------------------------

        /// <summary>
        /// Seeds the integration field with starting cells at their assigned costs.
        /// Returns the initial wavefront queue for Dijkstra expansion.
        ///
        /// For the goal sector: seeds = [goalCell] at cost 0.
        /// For corridor sectors: seeds = portal cells carrying costs from downstream.
        /// </summary>
        private static Queue<Vector2Int> SeedWavefront(FlowTile tile, Vector2Int[] seeds, ushort[] costs)
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

        private static int GetCostOutOfBoundsAsWalls(FlowTile tile, int x, int y) {
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

        /// <summary>
        /// Uses a modified Bresenham raycast to find cells blocked by a corner.
        /// </summary>
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

                // Stop if we hit another wall (except the starting corner itself)
                if (tile.Cost.IsWall(cx, cy))
                    break;

                ref var cell = ref tile.Integration[cx, cy];
                cell.Flags |= CellFlags.WaveFrontBlocked;
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
            return distance;
        }

        // -----------------------------------------------------------------------
        // Phase B: Line-of-Sight Pass
        // -----------------------------------------------------------------------

        /// <summary>
        /// Floods outward from the goal cell, marking cells that have unobstructed
        /// line-of-sight as HasLineOfSight.
        ///
        /// A cell has LOS if:
        ///   1. Its terrain cost is 1 (basic/flat terrain).
        ///   2. A Bresenham ray from the cell to the goal does not cross any wall.
        ///
        /// At visibility corners (cells adjacent to walls), we cast Bresenham rays
        /// and mark the first blocked cell as WaveFrontBlocked, creating the shadow
        /// boundary.
        ///
        /// Purpose: Agents in LOS cells steer directly toward the goal, eliminating
        /// the diamond-shaped artifacts that pure gradient descent creates near the
        /// destination.
        /// </summary>
        private static void ComputeLineOfSight(FlowTile tile, Vector2Int goalCell, Queue<Vector2Int> costWavefront)
        {
            int w = tile.Width;
            int h = tile.Height;

            // BFS flood from goal to find LOS region
            var visited = new bool[w * h];
            var queue = new Queue<Vector2Int>();

            // Mark goal cell as LOS
            ref var goalIntCell = ref tile.Integration[goalCell.x, goalCell.y];
            goalIntCell.Flags |= CellFlags.HasLineOfSight;
            visited[goalCell.y * w + goalCell.x] = true;
            queue.Enqueue(goalCell);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                ushort currentCost = tile.Integration[current.x, current.y].BestCost;

                // Expand to cardinal neighbors only (4-connected for LOS flood)
                foreach (var dir in Directions.Cardinal)
                {
                    var offset = Directions.Offset(dir);
                    var neighbor = current + offset;

                    if (neighbor.x < 0 || neighbor.x >= w || neighbor.y < 0 || neighbor.y >= h)
                        continue;

                    int neighborIdx = neighbor.y * w + neighbor.x;
                    ref var neighborInt = ref tile.Integration[neighbor.x, neighbor.y];

                    // If we hit an obstacle cell (cost > 1)
                    if (tile.Cost[neighbor.x, neighbor.y] > CostField.DefaultCost)
                    {
                        if (IsLosCorner(tile, current, neighbor)) 
                        {
                            CastShadowRay(tile, current, goalCell);
                        }
                        continue;
                    }

                    // If the neighbor is blocked by a shadow line, stop visibility propagation
                    // but seed it into the cost wavefront for Phase C cost integration.
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

                    // Increment the wave front cost by one (as the paper says) and flag as HasLineOfSight
                    neighborInt.BestCost = (ushort)(currentCost + 1);
                    neighborInt.Flags |= CellFlags.HasLineOfSight;
                    queue.Enqueue(neighbor);
                }
            }
        }

        // -----------------------------------------------------------------------
        // Phase C: Cost Integration
        // -----------------------------------------------------------------------

        /// <summary>
        /// Dijkstra-style wavefront expansion using 4 cardinal neighbors.
        ///
        /// Starting from the seeded cells, expands outward. For each neighbor:
        ///   newCost = cellCost + bestCostOfCurrentCell
        /// If newCost is lower than the neighbor's current BestCost, update and enqueue.
        ///
        /// The expansion stops at:
        ///   - Wall cells (CostField.Wall)
        ///   - Sector borders (out of bounds)
        ///   - Cells that already have a cheaper path
        /// </summary>
        private static void IntegrateCosts(FlowTile tile, Queue<Vector2Int> wavefront)
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

                    // Clamp to avoid overflow
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

        // -----------------------------------------------------------------------
        // Phase D: Build Flow Field
        // -----------------------------------------------------------------------

        /// <summary>
        /// For each reachable cell, determines the best flow direction by examining
        /// all 8 neighbors and picking the one with the lowest integrated cost.
        ///
        /// Cells with HasLineOfSight are marked with the LOS flag instead of a
        /// specific direction — agents in these cells should steer directly to the goal.
        ///
        /// Wall cells and unreachable cells get Direction.None.
        /// </summary>
        private static void BuildFlowField(FlowTile tile)
        {
            int w = tile.Width;
            int h = tile.Height;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    ref var integrationCell = ref tile.Integration[x, y];

                    // Walls and unreachable cells have no flow
                    if (tile.Cost.IsWall(x, y) || integrationCell.BestCost == IntegrationField.Unreachable)
                    {
                        tile.Flow[x, y] = FlowCell.FromDirection(Direction.None);
                        continue;
                    }

                    // LOS cells: agents steer directly to goal, no direction needed
                    if ((integrationCell.Flags & CellFlags.HasLineOfSight) != 0)
                    {
                        tile.Flow[x, y] = FlowCell.FromLineOfSight();
                        continue;
                    }

                    // Normal cell: find the neighbor with the lowest integrated cost
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

        // -----------------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------------

        /// <summary>
        /// Finds all portals in the corridor that connect the given sector
        /// to another corridor sector.
        /// </summary>
        private static List<Portal> FindCorridorPortals(PathRequest request, Vector2Int sector)
        {
            var result = new List<Portal>();
            foreach (var portal in request.Corridor)
            {
                if (portal.SectorA == sector || portal.SectorB == sector)
                {
                    var otherSector = portal.GetOtherSector(sector);
                    if (request.CorridorSectors.Contains(otherSector))
                        result.Add(portal);
                }
            }
            return result;
        }
    }
}
