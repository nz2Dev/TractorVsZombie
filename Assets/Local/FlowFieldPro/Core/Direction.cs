using UnityEngine;

namespace FlowFieldPro
{
    /// <summary>
    /// The 8 compass directions used for flow field navigation.
    /// Cardinal directions (N, E, S, W) are used during cost integration.
    /// All 8 directions are used when building the flow field.
    /// </summary>
    public enum Direction : byte
    {
        N   = 0,
        NE  = 1,
        E   = 2,
        SE  = 3,
        S   = 4,
        SW  = 5,
        W   = 6,
        NW  = 7,
        None = 8
    }

    /// <summary>
    /// Provides lookup tables and conversion utilities for <see cref="Direction"/>.
    /// </summary>
    public static class Directions
    {
        /// <summary>
        /// Offset vectors for all 8 directions, indexed by <see cref="Direction"/> ordinal.
        /// Order: N(0,1), NE(1,1), E(1,0), SE(1,-1), S(0,-1), SW(-1,-1), W(-1,0), NW(-1,1)
        /// </summary>
        public static readonly Vector2Int[] Offsets = new Vector2Int[]
        {
            new Vector2Int( 0,  1), // N
            new Vector2Int( 1,  1), // NE
            new Vector2Int( 1,  0), // E
            new Vector2Int( 1, -1), // SE
            new Vector2Int( 0, -1), // S
            new Vector2Int(-1, -1), // SW
            new Vector2Int(-1,  0), // W
            new Vector2Int(-1,  1), // NW
        };

        /// <summary>
        /// The 4 cardinal directions used during cost integration (Dijkstra wavefront).
        /// </summary>
        public static readonly Direction[] Cardinal = new Direction[]
        {
            Direction.N,
            Direction.E,
            Direction.S,
            Direction.W
        };

        /// <summary>
        /// All 8 directions used when building the flow field.
        /// </summary>
        public static readonly Direction[] All = new Direction[]
        {
            Direction.N,
            Direction.NE,
            Direction.E,
            Direction.SE,
            Direction.S,
            Direction.SW,
            Direction.W,
            Direction.NW
        };

        /// <summary>
        /// Returns the (dx, dy) offset for the given direction.
        /// </summary>
        public static Vector2Int Offset(Direction direction)
        {
            if (direction == Direction.None)
                return Vector2Int.zero;
            return Offsets[(int)direction];
        }

        /// <summary>
        /// Finds the <see cref="Direction"/> that matches the given offset vector.
        /// Returns <see cref="Direction.None"/> if the offset doesn't match any direction.
        /// </summary>
        public static Direction FromOffset(Vector2Int offset)
        {
            for (int i = 0; i < Offsets.Length; i++)
            {
                if (Offsets[i] == offset)
                    return (Direction)i;
            }
            return Direction.None;
        }
    }
}
