using System.Collections.Generic;
using UnityEngine;

namespace FlowFieldPro
{
    /// <summary>
    /// A* pathfinder on the portal graph.
    ///
    /// Runs BACKWARD from the goal sector: finds which portals must be traversed
    /// for any source to reach the goal. This "reverse search" naturally supports
    /// multiple starting positions sharing a single goal — all sources that fall
    /// within the explored corridor get their paths for free.
    ///
    /// The output is a sector corridor: a set of portals (and their sectors)
    /// through which agents must travel.
    /// </summary>
    public static class PortalAStar
    {
        /// <summary>
        /// Finds the portal corridor from the goal to all reachable sources.
        ///
        /// Algorithm:
        /// 1. Start with the goal sector. Find all portals in that sector.
        /// 2. Run A* backward through the portal graph, expanding toward source sectors.
        /// 3. Stop when all source sectors have been reached (or the graph is exhausted).
        /// 4. Collect the set of portals and sectors that form the corridor.
        /// </summary>
        public static void FindCorridor(PathRequest request, SectorGrid grid, PortalGraph portalGraph)
        {
            var goalSector = grid.WorldToSector(request.Goal);

            // Collect distinct source sectors
            var sourceSectors = new HashSet<Vector2Int>();
            foreach (var source in request.Sources)
                sourceSectors.Add(grid.WorldToSector(source));

            // If all sources are in the goal sector, no portal traversal needed
            if (sourceSectors.Count == 1 && sourceSectors.Contains(goalSector))
            {
                request.CorridorSectors.Add(goalSector);
                return;
            }

            request.CorridorSectors.Add(goalSector);

            // A* data structures
            // We use a simple sorted list as a priority queue (sufficient for portal graph sizes)
            var openSet = new SortedList<float, Portal>(new DuplicateKeyComparer());
            var cameFrom = new Dictionary<int, Portal>();   // portal.Id -> previous portal
            var gScore = new Dictionary<int, float>();      // portal.Id -> cost from goal
            var closedSet = new HashSet<int>();

            // Seed: all portals in the goal sector, with distance from goal to each portal's cells
            var goalPortals = portalGraph.GetPortalsInSector(goalSector);
            foreach (var portal in goalPortals)
            {
                // Approximate cost = Manhattan distance from goal to portal center
                var goalLocal = grid.WorldToLocal(request.Goal);
                var portalLocal = grid.WorldToLocal(portal.CenterWorld);
                float startCost = ManhattanDistance(goalLocal, portalLocal);

                gScore[portal.Id] = startCost;
                float heuristic = HeuristicToAnySector(portal.CenterWorld, sourceSectors, grid);
                openSet.Add(startCost + heuristic, portal);
            }

            // Track which source sectors have been reached
            var reachedSources = new HashSet<Vector2Int>();

            while (openSet.Count > 0)
            {
                // Pop lowest f-score portal
                var currentPortal = openSet.Values[0];
                openSet.RemoveAt(0);

                if (closedSet.Contains(currentPortal.Id))
                    continue;
                closedSet.Add(currentPortal.Id);

                // Mark both sectors of this portal as part of the corridor
                request.CorridorSectors.Add(currentPortal.SectorA);
                request.CorridorSectors.Add(currentPortal.SectorB);
                request.Corridor.Add(currentPortal);

                // Check if we reached a source sector
                if (sourceSectors.Contains(currentPortal.SectorA))
                    reachedSources.Add(currentPortal.SectorA);
                if (sourceSectors.Contains(currentPortal.SectorB))
                    reachedSources.Add(currentPortal.SectorB);

                // If all sources have been reached, we're done
                if (reachedSources.Count >= sourceSectors.Count)
                    break;

                // Expand neighbors in the portal graph
                float currentG = gScore[currentPortal.Id];
                var neighbors = portalGraph.GetNeighbors(currentPortal);

                foreach (var edge in neighbors)
                {
                    if (closedSet.Contains(edge.Target.Id))
                        continue;

                    float tentativeG = currentG + edge.Cost;

                    if (!gScore.ContainsKey(edge.Target.Id) || tentativeG < gScore[edge.Target.Id])
                    {
                        gScore[edge.Target.Id] = tentativeG;
                        cameFrom[edge.Target.Id] = currentPortal;
                        float h = HeuristicToAnySector(edge.Target.CenterWorld, sourceSectors, grid);
                        openSet.Add(tentativeG + h, edge.Target);
                    }
                }
            }
        }

        /// <summary>
        /// Heuristic: minimum Manhattan distance from a position to any source sector's center.
        /// </summary>
        private static float HeuristicToAnySector(Vector2Int position, HashSet<Vector2Int> sourceSectors, SectorGrid grid)
        {
            float minDist = float.MaxValue;
            foreach (var sector in sourceSectors)
            {
                // Use sector center as target for heuristic
                var sectorCenter = new Vector2Int(
                    sector.x * grid.SectorSize + grid.SectorSize / 2,
                    sector.y * grid.SectorSize + grid.SectorSize / 2
                );
                float dist = ManhattanDistance(position, sectorCenter);
                if (dist < minDist)
                    minDist = dist;
            }
            return minDist;
        }

        private static float ManhattanDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        /// <summary>
        /// Comparer that allows duplicate keys in SortedList by breaking ties with a counter.
        /// This lets us use SortedList as a min-heap / priority queue.
        /// </summary>
        private class DuplicateKeyComparer : IComparer<float>
        {
            public int Compare(float x, float y)
            {
                int result = x.CompareTo(y);
                // If equal, return 1 so that duplicate keys are allowed
                return result == 0 ? 1 : result;
            }
        }
    }
}
