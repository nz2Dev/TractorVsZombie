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
                        IntegrateTile(neighborTile, seedCells.ToArray(), seedCosts.ToArray(), grid, neighborSector);

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
            var wavefront = SeedWavefrontPass.SeedWavefront(tile, seedCells, seedCosts);

            // Phase B: LOS pass (only for goal-sector tiles where seeds start at cost 0)
            bool isGoalTile = seedCosts == null;
            if (isGoalTile && seedCells.Length == 1)
                LineOfSightPass.ComputeLineOfSight(tile, seedCells[0], wavefront);

            // Phase C: Cost integration (Dijkstra expansion)
            CostIntegrationPass.IntegrateCosts(tile, wavefront);

            // Phase D: Build flow directions
            FlowFieldBuilderPass.BuildFlowField(tile);
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
