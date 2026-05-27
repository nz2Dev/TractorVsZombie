using UnityEngine;

namespace FlowFieldPro
{
    /// <summary>
    /// Represents a traversable opening on a sector border.
    ///
    /// A portal is a contiguous run of non-wall cells along the shared edge
    /// between two adjacent sectors. Portals serve as nodes in the high-level
    /// portal graph used for A* corridor pathfinding.
    ///
    /// Each portal stores the border cells on both sides so the TileIntegrator
    /// can seed these cells when building flow tiles for corridor sectors.
    /// </summary>
    public class Portal
    {
        /// <summary>Unique identifier within the portal graph.</summary>
        public int Id;

        /// <summary>The first sector this portal connects (lower coordinate).</summary>
        public Vector2Int SectorA;

        /// <summary>The second sector this portal connects (higher coordinate).</summary>
        public Vector2Int SectorB;

        /// <summary>
        /// World-space cells on sector A's border edge that belong to this portal.
        /// </summary>
        public Vector2Int[] WorldCellsA;

        /// <summary>
        /// World-space cells on sector B's border edge that belong to this portal.
        /// </summary>
        public Vector2Int[] WorldCellsB;

        /// <summary>
        /// The midpoint of the portal window in world-space coordinates.
        /// Used as the A* node position for heuristic calculations.
        /// </summary>
        public Vector2Int CenterWorld;

        /// <summary>
        /// Returns the sector on the other side of this portal from the given sector.
        /// </summary>
        public Vector2Int GetOtherSector(Vector2Int sector)
        {
            return sector == SectorA ? SectorB : SectorA;
        }

        /// <summary>
        /// Returns the world cells on the given sector's side of the portal.
        /// </summary>
        public Vector2Int[] GetCellsForSector(Vector2Int sector)
        {
            return sector == SectorA ? WorldCellsA : WorldCellsB;
        }
    }
}
