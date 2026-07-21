using System.Collections.Generic;

using UnityEngine;

public static partial class FlowFieldIntegrator {
    
    public static class CostIntegrationPass {
        
        internal static void ComputeCosts(FlowField field, Vector2Int goal, IEnumerable<Vector2Int> wavefrontInput) {
            for (int x = 0; x < field.Size; x++) {
                for (int y = 0; y < field.Size; y++) {
                    field[x, y].integratedCost = 0;
                }
            }

            var inSearch = new Queue<Vector2Int>();
            foreach (var cell in wavefrontInput) {
                inSearch.Enqueue(cell);
            }

            while (inSearch.Count > 0) {    
                var nextLocation = inSearch.Dequeue();
                var nextCell = field[nextLocation];

                foreach (var direction in Directions.Cardinal) {
                    var neighborLocation = nextLocation + Directions.Offset(direction);
                    
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
}