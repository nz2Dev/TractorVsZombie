using System;

namespace FlowFieldPro
{
    /// <summary>
    /// Stores the traversal cost for each cell in a sector.
    /// Costs range from 1 (easily traversable) to 254 (very expensive).
    /// A cost of 255 marks the cell as impassable (wall).
    ///
    /// Data is stored as a flat row-major byte array for cache-friendly access.
    /// The cost field is generated offline from terrain, slopes, and obstacles,
    /// and provided to the library from outside.
    /// </summary>
    public class CostField
    {
        public const byte DefaultCost = 1;
        public const byte Wall = 255;

        private readonly byte[] costs;
        private readonly int width;
        private readonly int height;

        public int Width => width;
        public int Height => height;

        /// <summary>
        /// Creates a cost field with all cells set to <see cref="DefaultCost"/>.
        /// </summary>
        public CostField(int width, int height)
        {
            this.width = width;
            this.height = height;
            costs = new byte[width * height];

            for (int i = 0; i < costs.Length; i++)
                costs[i] = DefaultCost;
        }

        /// <summary>
        /// Creates a cost field from externally provided data.
        /// The array must be exactly width * height bytes, in row-major order.
        /// </summary>
        public CostField(int width, int height, byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length != width * height)
                throw new ArgumentException($"Data length {data.Length} does not match {width}x{height}={width * height}");

            this.width = width;
            this.height = height;
            costs = new byte[data.Length];
            Array.Copy(data, costs, data.Length);
        }

        public byte this[int x, int y]
        {
            get => costs[y * width + x];
            set => costs[y * width + x] = value;
        }

        public bool IsWall(int x, int y)
        {
            return costs[y * width + x] == Wall;
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public void SetWall(int x, int y)
        {
            costs[y * width + x] = Wall;
        }

        public void Clear(byte cost = DefaultCost)
        {
            for (int i = 0; i < costs.Length; i++)
                costs[i] = cost;
        }
    }
}
