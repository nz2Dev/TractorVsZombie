using System;

namespace FlowFieldPro
{
    /// <summary>
    /// Flags assigned to integration cells during tile construction.
    /// These flags control how the flow field and LOS pass interact.
    /// </summary>
    [Flags]
    public enum CellFlags : byte
    {
        None             = 0,
        /// <summary>Cell is in the active wavefront queue during integration.</summary>
        ActiveWaveFront  = 1 << 0,
        /// <summary>Cell has unobstructed line-of-sight to the goal.</summary>
        HasLineOfSight   = 1 << 1,
        /// <summary>Cell was reached by a Bresenham ray that was then blocked (corner shadow boundary).</summary>
        WaveFrontBlocked = 1 << 2,
    }

    /// <summary>
    /// Per-cell integration data: the accumulated cost-to-goal and processing flags.
    /// </summary>
    public struct IntegrationCell
    {
        public double BestCost;
        public CellFlags Flags;
    }

    /// <summary>
    /// Stores the integration field for a single sector.
    /// Each cell holds the cheapest accumulated cost to reach the goal,
    /// computed via Dijkstra-style wavefront expansion from seed cells.
    ///
    /// The integration field is the bridge between the cost field (static terrain data)
    /// and the flow field (per-cell steering directions).
    /// </summary>
    public class IntegrationField
    {
        /// <summary>
        /// Sentinel value meaning "this cell has not been reached by the wavefront".
        /// Any cell with this cost is either a wall or unreachable.
        /// </summary>
        public const double Unreachable = double.MaxValue;

        private readonly IntegrationCell[] cells;
        private readonly int width;
        private readonly int height;

        public int Width => width;
        public int Height => height;

        public IntegrationField(int width, int height)
        {
            this.width = width;
            this.height = height;
            cells = new IntegrationCell[width * height];
            Reset();
        }

        public ref IntegrationCell this[int x, int y]
        {
            get => ref cells[y * width + x];
        }

        /// <summary>
        /// Resets all cells to <see cref="Unreachable"/> cost and clears all flags.
        /// Must be called before each integration pass.
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].BestCost = Unreachable;
                cells[i].Flags = CellFlags.None;
            }
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
    }
}
