using System.Collections.Generic;
using UnityEngine;

namespace FlowFieldPro
{
    /// <summary>
    /// Edge in the portal graph connecting two portals with a traversal cost.
    /// </summary>
    public struct PortalEdge
    {
        public Portal Target;
        public int Cost;
    }

    /// <summary>
    /// Builds and stores the high-level navigation graph used for A* pathfinding.
    ///
    /// The graph has two types of edges:
    /// - Inter-sector edges: connect a portal's cell pairs across the sector border (cost = 1).
    /// - Intra-sector edges: connect portals within the same sector, weighted by
    ///   the BFS shortest path distance between their cells through the sector's cost field.
    ///
    /// Portal detection:
    /// Walk each shared border between adjacent sectors. A contiguous run of cells
    /// that are non-wall on BOTH sides forms a single portal window.
    /// </summary>
    public class PortalGraph
    {
        private readonly List<Portal> allPortals = new List<Portal>();
        private readonly Dictionary<int, List<PortalEdge>> adjacency = new Dictionary<int, List<PortalEdge>>();

        // Quick lookup: which portals belong to a given sector
        private readonly Dictionary<Vector2Int, List<Portal>> portalsBySector = new Dictionary<Vector2Int, List<Portal>>();

        public IReadOnlyList<Portal> AllPortals => allPortals;

        public List<Portal> GetPortalsInSector(Vector2Int sector)
        {
            if (portalsBySector.TryGetValue(sector, out var list))
                return list;
            return new List<Portal>();
        }

        public List<PortalEdge> GetNeighbors(Portal portal)
        {
            if (adjacency.TryGetValue(portal.Id, out var edges))
                return edges;
            return new List<PortalEdge>();
        }

        /// <summary>
        /// Scans all sector borders in the grid, detects portal windows,
        /// and builds the full adjacency graph with intra-sector edge weights.
        /// </summary>
        public void BuildFromSectorGrid(SectorGrid grid)
        {
            allPortals.Clear();
            adjacency.Clear();
            portalsBySector.Clear();

            int nextId = 0;

            // Scan horizontal borders (between sector rows)
            for (int sx = 0; sx < grid.SectorsX; sx++)
            {
                for (int sy = 0; sy < grid.SectorsY - 1; sy++)
                {
                    var sectorA = new Vector2Int(sx, sy);
                    var sectorB = new Vector2Int(sx, sy + 1);
                    var portals = DetectBorderPortals(grid, sectorA, sectorB, BorderDirection.Horizontal, ref nextId);
                    RegisterPortals(portals);
                }
            }

            // Scan vertical borders (between sector columns)
            for (int sy = 0; sy < grid.SectorsY; sy++)
            {
                for (int sx = 0; sx < grid.SectorsX - 1; sx++)
                {
                    var sectorA = new Vector2Int(sx, sy);
                    var sectorB = new Vector2Int(sx + 1, sy);
                    var portals = DetectBorderPortals(grid, sectorA, sectorB, BorderDirection.Vertical, ref nextId);
                    RegisterPortals(portals);
                }
            }

            // Build intra-sector edges between portals in the same sector
            BuildIntraSectorEdges(grid);
        }

        private enum BorderDirection { Horizontal, Vertical }

        /// <summary>
        /// Walks a shared border between two adjacent sectors and groups contiguous
        /// non-wall cell pairs into portal windows.
        ///
        /// For a horizontal border (sectorA below, sectorB above):
        ///   - A's top row (local y = sectorHeight-1)
        ///   - B's bottom row (local y = 0)
        ///
        /// For a vertical border (sectorA left, sectorB right):
        ///   - A's right column (local x = sectorWidth-1)
        ///   - B's left column (local x = 0)
        /// </summary>
        private List<Portal> DetectBorderPortals(
            SectorGrid grid, Vector2Int sectorA, Vector2Int sectorB,
            BorderDirection border, ref int nextId)
        {
            var result = new List<Portal>();

            int length; // how many cells along the shared border
            if (border == BorderDirection.Horizontal)
                length = Mathf.Min(grid.GetSectorWidth(sectorA.x), grid.GetSectorWidth(sectorB.x));
            else
                length = Mathf.Min(grid.GetSectorHeight(sectorA.y), grid.GetSectorHeight(sectorB.y));

            var costA = grid.GetCostField(sectorA);
            var costB = grid.GetCostField(sectorB);

            var runCellsA = new List<Vector2Int>();
            var runCellsB = new List<Vector2Int>();

            for (int i = 0; i < length; i++)
            {
                Vector2Int localA, localB;
                if (border == BorderDirection.Horizontal)
                {
                    // A's top row, B's bottom row
                    localA = new Vector2Int(i, costA.Height - 1);
                    localB = new Vector2Int(i, 0);
                }
                else
                {
                    // A's right column, B's left column
                    localA = new Vector2Int(costA.Width - 1, i);
                    localB = new Vector2Int(0, i);
                }

                bool passable = !costA.IsWall(localA.x, localA.y)
                             && !costB.IsWall(localB.x, localB.y);

                if (passable)
                {
                    runCellsA.Add(grid.LocalToWorld(sectorA, localA));
                    runCellsB.Add(grid.LocalToWorld(sectorB, localB));
                }
                else
                {
                    // End of a contiguous run — emit portal if we have cells
                    if (runCellsA.Count > 0)
                    {
                        result.Add(CreatePortal(sectorA, sectorB, runCellsA, runCellsB, ref nextId));
                        runCellsA = new List<Vector2Int>();
                        runCellsB = new List<Vector2Int>();
                    }
                }
            }

            // Emit final run
            if (runCellsA.Count > 0)
                result.Add(CreatePortal(sectorA, sectorB, runCellsA, runCellsB, ref nextId));

            return result;
        }

        private Portal CreatePortal(
            Vector2Int sectorA, Vector2Int sectorB,
            List<Vector2Int> cellsA, List<Vector2Int> cellsB,
            ref int nextId)
        {
            var portal = new Portal
            {
                Id = nextId++,
                SectorA = sectorA,
                SectorB = sectorB,
                WorldCellsA = cellsA.ToArray(),
                WorldCellsB = cellsB.ToArray(),
            };

            // Center is the midpoint of side A cells (representative position for heuristics)
            var first = cellsA[0];
            var last = cellsA[cellsA.Count - 1];
            portal.CenterWorld = new Vector2Int((first.x + last.x) / 2, (first.y + last.y) / 2);

            return portal;
        }

        private void RegisterPortals(List<Portal> portals)
        {
            foreach (var portal in portals)
            {
                allPortals.Add(portal);
                adjacency[portal.Id] = new List<PortalEdge>();

                RegisterInSector(portal.SectorA, portal);
                RegisterInSector(portal.SectorB, portal);
            }
        }

        private void RegisterInSector(Vector2Int sector, Portal portal)
        {
            if (!portalsBySector.TryGetValue(sector, out var list))
            {
                list = new List<Portal>();
                portalsBySector[sector] = list;
            }
            list.Add(portal);
        }

        /// <summary>
        /// For each sector, compute BFS distances between all pairs of portals
        /// and create weighted intra-sector edges.
        ///
        /// Uses multi-source BFS from each portal's cells within the sector's cost field.
        /// </summary>
        private void BuildIntraSectorEdges(SectorGrid grid)
        {
            foreach (var kvp in portalsBySector)
            {
                var sector = kvp.Key;
                var portals = kvp.Value;

                if (portals.Count < 2)
                    continue;

                var costField = grid.GetCostField(sector);
                int w = costField.Width;
                int h = costField.Height;

                // For each portal, BFS to find distances to all other portals in the sector
                for (int i = 0; i < portals.Count; i++)
                {
                    var sourcePortal = portals[i];
                    var distances = BFSFromPortal(grid, sector, sourcePortal, costField, w, h);

                    for (int j = i + 1; j < portals.Count; j++)
                    {
                        var targetPortal = portals[j];
                        int dist = GetMinDistanceToPortal(grid, sector, targetPortal, distances, w);

                        if (dist < int.MaxValue)
                        {
                            adjacency[sourcePortal.Id].Add(new PortalEdge { Target = targetPortal, Cost = dist });
                            adjacency[targetPortal.Id].Add(new PortalEdge { Target = sourcePortal, Cost = dist });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// BFS from a portal's cells within a sector. Returns a flat distance array
        /// in local sector coordinates.
        /// </summary>
        private int[] BFSFromPortal(SectorGrid grid, Vector2Int sector, Portal portal, CostField costField, int w, int h)
        {
            int[] dist = new int[w * h];
            for (int i = 0; i < dist.Length; i++)
                dist[i] = int.MaxValue;

            var queue = new Queue<Vector2Int>();

            // Seed with the portal's cells that belong to this sector
            var portalCells = portal.GetCellsForSector(sector);
            foreach (var worldCell in portalCells)
            {
                var local = grid.WorldToLocal(worldCell);
                int idx = local.y * w + local.x;
                dist[idx] = 0;
                queue.Enqueue(local);
            }

            // Standard BFS with 4-connected neighbors
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                int currentIdx = current.y * w + current.x;
                int currentDist = dist[currentIdx];

                foreach (var dir in Directions.Cardinal)
                {
                    var offset = Directions.Offset(dir);
                    var neighbor = current + offset;

                    if (neighbor.x < 0 || neighbor.x >= w || neighbor.y < 0 || neighbor.y >= h)
                        continue;
                    if (costField.IsWall(neighbor.x, neighbor.y))
                        continue;

                    int neighborIdx = neighbor.y * w + neighbor.x;
                    int newDist = currentDist + costField[neighbor.x, neighbor.y];

                    if (newDist < dist[neighborIdx])
                    {
                        dist[neighborIdx] = newDist;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return dist;
        }

        /// <summary>
        /// Returns the minimum BFS distance from a source portal to any cell
        /// of the target portal within the same sector.
        /// </summary>
        private int GetMinDistanceToPortal(SectorGrid grid, Vector2Int sector, Portal targetPortal, int[] distances, int w)
        {
            int minDist = int.MaxValue;
            var targetCells = targetPortal.GetCellsForSector(sector);

            foreach (var worldCell in targetCells)
            {
                var local = grid.WorldToLocal(worldCell);
                int idx = local.y * w + local.x;
                if (distances[idx] < minDist)
                    minDist = distances[idx];
            }

            return minDist;
        }
    }
}
