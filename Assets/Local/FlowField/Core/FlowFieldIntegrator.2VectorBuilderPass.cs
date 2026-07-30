using UnityEngine;

public static partial class FlowFieldIntegrator {

    public static class VectorBuilderPass {

        internal static void ComputeFlow(FlowField field, Vector2Int goal) {
            for (int x = 0; x < field.Size; x++) {
                for (int y = 0; y < field.Size; y++) {
                    var cellLocation = new Vector2Int(x, y);
                    ref var cell = ref field.GetRef(x, y);
                    if (cell.IsBlocked()) {
                        cell.flowVector = Vector2Int.zero;
                        continue;
                    }

                    var lowestCost = int.MaxValue;
                    Vector2Int lowestCostLocation = new Vector2Int(x, y);
                    foreach (var direction in Directions.All) {
                        var neighborLocation = cellLocation + Directions.Offset(direction);
                        
                        if (!field.IsInBounds(neighborLocation))
                            continue;

                        ref var neighborCell = ref field.GetRef(neighborLocation);
                        if (neighborCell.IsBlocked() || neighborCell.integratedCost == 0 && neighborLocation != goal)
                            continue;

                        if (neighborCell.integratedCost < lowestCost) {
                            lowestCost = neighborCell.integratedCost;
                            lowestCostLocation = neighborLocation;
                        }
                    }

                    cell.flowVector = lowestCostLocation - cellLocation;
                }
            }
        }
    }
}