using System;
using System.Collections.Generic;

using UnityEngine;

public static partial class FlowFieldIntegrator {

    public static class LineOfSightPass {

        public static void ComputeLineOfSight(FlowField field, Vector2Int goal, List<Vector2Int> wavefrontOutput) {
            using (FlowFieldProfiling.LineOfSightMarker.Auto()) {
                ref var goalCell = ref field.GetRef(goal);
                goalCell.SetFlag(CellFlags.HasLineOfSight);
                goalCell.integratedCost = 0;
                
                var queue = new Queue<Vector2Int>();
                queue.Enqueue(goal);
                while (queue.Count > 0) {
                    var current = queue.Dequeue();
                    ref var currentCell = ref field.GetRef(current);

                    if (CornerDetector.IsLosCorner(field, current, goal)) {
                        ShadowCaster.CastShadowRay(field, current, goal);
                        currentCell.UnsetFlag(CellFlags.HasLineOfSight);
                        wavefrontOutput.Add(current);
                        continue;
                    }

                    if (currentCell.HasFlag(CellFlags.WaveFrontBlocked)) {
                        currentCell.UnsetFlag(CellFlags.HasLineOfSight);
                        wavefrontOutput.Add(current);
                        continue;
                    }

                    foreach (var direction in Directions.Cardinal) {
                        var neighbor = current + Directions.Offset(direction);

                        if (!field.IsInBounds(neighbor.x, neighbor.y))
                            continue;

                        ref var neighborCell = ref field.GetRef(neighbor);
                        if (neighborCell.cost > Cell.DefaultCost || neighborCell.integratedCost != 0)
                            continue;

                        neighborCell.integratedCost = currentCell.integratedCost + 1;
                        if (neighborCell.HasFlag(CellFlags.WaveFrontBlocked)) {
                            wavefrontOutput.Add(neighbor);
                            continue;
                        }

                        neighborCell.SetFlag(CellFlags.HasLineOfSight);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
    }
}