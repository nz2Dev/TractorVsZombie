namespace FlowFieldPro
{
    /// <summary>
    /// Bundles the three per-sector field layers into a single unit of work.
    ///
    /// A FlowTile represents one sector's complete pathfinding state:
    /// - CostField:        static terrain costs (provided from outside)
    /// - IntegrationField: accumulated cost-to-goal (computed by TileIntegrator)
    /// - FlowField:        per-cell steering directions (computed by TileIntegrator)
    ///
    /// The TileIntegrator processes a FlowTile through 4 phases:
    /// seed → LOS → integration → flow.
    /// </summary>
    public class FlowTile
    {
        public readonly CostField Cost;
        public readonly IntegrationField Integration;
        public readonly FlowField Flow;

        public int Width => Cost.Width;
        public int Height => Cost.Height;

        public FlowTile(CostField costField)
        {
            Cost = costField;
            Integration = new IntegrationField(costField.Width, costField.Height);
            Flow = new FlowField(costField.Width, costField.Height);
        }

        /// <summary>
        /// Creates a FlowTile with a default (all traversable) cost field.
        /// </summary>
        public FlowTile(int width, int height)
            : this(new CostField(width, height))
        {
        }

        /// <summary>
        /// Resets integration and flow data while preserving the cost field.
        /// Call this before re-integrating with a new goal or portal seed.
        /// </summary>
        public void ResetComputed()
        {
            Integration.Reset();
            Flow.Clear();
        }
    }
}
