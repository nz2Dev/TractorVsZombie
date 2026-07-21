using UnityEngine;

public static class FlowFieldVectorBuilderPass {
    
    public static readonly Vector2Int[] FlowNeighborsOffsets = new Vector2Int[] {
        new(0, -1),
        new(+1, -1),
        new(+1, 0),
        new(+1, +1),
        new(0, +1),
        new(-1, +1),
        new(-1, 0),
        new(-1, -1),
    };
    
    internal static void ComputeFlow(FlowField field, Vector2Int goal) {
        for (int x = 0; x < field.Size; x++) {
            for (int y = 0; y < field.Size; y++) {
                var cellLocation = new Vector2Int(x, y);
                var cell = field[cellLocation];
                if (cell.IsBlocked()) {
                    field[x, y].flowVector = Vector2Int.zero;
                    continue;
                }
                
                var lowestCost = int.MaxValue;
                Vector2Int lowestCostLocation = new Vector2Int(x, y);
                foreach (var offset in FlowNeighborsOffsets) {
                    var neighborLocation = cellLocation + offset;
                    if (!field.IsInBounds(neighborLocation))
                        continue;
                    
                    var neighborCell = field[neighborLocation];
                    if (neighborCell.IsBlocked() || neighborCell.integratedCost == 0 && neighborLocation != goal)
                        continue;

                    if (neighborCell.integratedCost < lowestCost) {
                        lowestCost = neighborCell.integratedCost;
                        lowestCostLocation = neighborLocation;
                    }
                }

                field[x, y].flowVector = lowestCostLocation - cellLocation;
            }
        }
    }

}