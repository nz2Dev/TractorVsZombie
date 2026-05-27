using System.Collections.Generic;
using UnityEngine;

namespace FlowFieldPro
{
    /// <summary>
    /// Represents a single pathfinding request.
    ///
    /// A path request has one goal and potentially multiple starting positions.
    /// The A* runs backward from the goal through the portal graph, producing
    /// a sector corridor. The TileIntegrator then builds flow tiles for each
    /// sector in the corridor.
    ///
    /// After processing, agents at any source position can read their flow tile
    /// to get a steering direction.
    /// </summary>
    public class PathRequest
    {
        /// <summary>The single goal cell in world coordinates.</summary>
        public Vector2Int Goal;

        /// <summary>One or more starting positions in world coordinates.</summary>
        public Vector2Int[] Sources;

        /// <summary>
        /// The ordered sector corridor produced by portal A*.
        /// Contains the sequence of portals from goal outward to all sources.
        /// </summary>
        public List<Portal> Corridor;

        /// <summary>
        /// The set of sector coordinates that are part of the corridor.
        /// </summary>
        public HashSet<Vector2Int> CorridorSectors;

        /// <summary>
        /// Computed flow tiles indexed by sector coordinate.
        /// Each tile contains the integrated cost-to-goal and per-cell flow directions.
        /// </summary>
        public Dictionary<Vector2Int, FlowTile> Tiles;

        public PathRequest(Vector2Int goal, params Vector2Int[] sources)
        {
            Goal = goal;
            Sources = sources;
            Corridor = new List<Portal>();
            CorridorSectors = new HashSet<Vector2Int>();
            Tiles = new Dictionary<Vector2Int, FlowTile>();
        }
    }
}
