using System.Collections.Generic;

using UnityEngine;

public static class FlowFieldCostIntegrationPass {

    public static readonly Vector2Int[] CostNeighborsOffsets = new Vector2Int[] {
        new(0, -1),
        new(+1, 0),
        new(0, +1),
        new(-1, 0),
    };
    
    internal static void ComputeCosts(FlowField field, Vector2Int goal) {
        for (int x = 0; x < field.Size; x++) {
            for (int y = 0; y < field.Size; y++) {
                field[x, y].integratedCost = 0;
            }
        }

        var inSearch = new Queue<Vector2Int>();
        inSearch.Enqueue(goal);
        while (inSearch.Count > 0) {    
            var nextLocation = inSearch.Dequeue();
            var nextCell = field[nextLocation];

            foreach (var offset in CostNeighborsOffsets) {
                var neighborLocation = nextLocation + offset;
                if (!field.IsInBounds(neighborLocation) || neighborLocation == goal)
                    continue;

                var neighborCell = field[neighborLocation];
                if (neighborCell.IsBlocked() || neighborCell.integratedCost != 0)
                    continue;

                neighborCell.integratedCost = neighborCell.cost + nextCell.integratedCost;
                inSearch.Enqueue(neighborLocation);
            }
        }
    }
}