using System.Collections.Generic;

using UnityEngine;

public static partial class FlowFieldIntegrator {

    internal struct CostNode {
        public Vector2Int cell;
        public int distance;
    }

    internal class CostNodeComparer : INodeComparer<CostNode> {
        public static CostNodeComparer Instance = new ();
        public bool FirstIsLess(CostNode first, CostNode second) => first.distance < second.distance;
        public bool FirstIsLessEqual(CostNode first, CostNode second) => first.distance <= second.distance;
    }

    public static class CostIntegrationPass {
        
        internal static void ComputeCosts(FlowField field, Vector2Int goal, IEnumerable<Vector2Int> wavefrontInput) {
            var inSearch = new MinHeap<CostNode>(field.Size, CostNodeComparer.Instance);
            foreach (var cell in wavefrontInput) {
                inSearch.Push(new CostNode { cell = cell, distance = field[cell.x, cell.y].integratedCost });
            }

            while (inSearch.Count > 0) {    
                var nextLocation = inSearch.Pop().cell;
                ref var nextCell = ref field.GetRef(nextLocation);

                foreach (var direction in Directions.Cardinal) {
                    var neighborLocation = nextLocation + Directions.Offset(direction);
                    
                    if (!field.IsInBounds(neighborLocation))
                        continue;

                    ref var neighborCell = ref field.GetRef(neighborLocation);
                    if (neighborCell.IsBlocked() || neighborCell.HasFlag(CellFlags.HasLineOfSight))
                        continue;

                    var newCost = neighborCell.cost + nextCell.integratedCost;
                    if (neighborCell.integratedCost == 0 || newCost < neighborCell.integratedCost) {
                        neighborCell.integratedCost = newCost;
                        inSearch.Push(new CostNode { cell = neighborLocation, distance = neighborCell.integratedCost });
                    }
                }
            }
        }
    }
}