namespace FlowFieldPro
{
    /// <summary>
    /// A single flow cell, packed into one byte for compactness.
    /// Low 4 bits  = <see cref="Direction"/> index (0–8).
    /// Bit 4       = HasLineOfSight flag.
    ///
    /// When HasLineOfSight is set, agents should steer directly toward the goal
    /// instead of following the flow direction. This eliminates the diamond-shaped
    /// artifacts that appear near the destination with pure gradient descent.
    /// </summary>
    public struct FlowCell
    {
        public byte Packed;

        public Direction Direction => (Direction)(Packed & 0x0F);
        public bool HasLineOfSight => (Packed & 0x10) != 0;

        public static FlowCell FromDirection(Direction direction)
        {
            return new FlowCell { Packed = (byte)direction };
        }

        public static FlowCell FromLineOfSight()
        {
            return new FlowCell { Packed = (byte)((byte)Direction.None | 0x10) };
        }
    }

    /// <summary>
    /// Stores the flow field for a single sector.
    /// Each cell contains a packed direction that points toward the steepest descent
    /// in the integration field (i.e., toward the goal).
    ///
    /// This is the final output that agents read at runtime.
    /// One flow field can steer arbitrarily many agents through the same sector.
    /// </summary>
    public class FlowField
    {
        private readonly FlowCell[] cells;
        private readonly int width;
        private readonly int height;

        public int Width => width;
        public int Height => height;

        public FlowField(int width, int height)
        {
            this.width = width;
            this.height = height;
            cells = new FlowCell[width * height];
        }

        public FlowCell this[int x, int y]
        {
            get => cells[y * width + x];
            set => cells[y * width + x] = value;
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public void Clear()
        {
            for (int i = 0; i < cells.Length; i++)
                cells[i] = default;
        }
    }
}
