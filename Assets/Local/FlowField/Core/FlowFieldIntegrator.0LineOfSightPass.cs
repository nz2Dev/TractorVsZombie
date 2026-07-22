using System;
using System.Collections.Generic;

using UnityEngine;

public static partial class FlowFieldIntegrator {

    public static class LineOfSightPass {

        public static void ComputeLineOfSight(FlowField field, Vector2Int goal, List<Vector2Int> wavefrontOutput) {
            var goalCell = field[goal.x, goal.y]; // ref was used here
            goalCell.SetFlag(CellFlags.HasLineOfSight);
            goalCell.integratedCost = 0;
            
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(goal);
            while (queue.Count > 0) {
                StepLineOfSight(queue, field, goal, wavefrontOutput);
            }
        }

        internal static void StepLineOfSight(Queue<Vector2Int> queue, FlowField field, Vector2Int goal, List<Vector2Int> wavefrontOutput) {
            var current = queue.Dequeue();
            var currentCell = field[current.x, current.y]; // ref was used here

            bool isLosCorner = false;
            foreach (var direction in Directions.Cardinal) {
                var neighbor = current + Directions.Offset(direction);

                if (!field.IsInBounds(neighbor.x, neighbor.y))
                    continue;

                if (field[neighbor.x, neighbor.y].cost > Cell.DefaultCost) {
                    if (CornerDetector.IsLosCorner(field, current, neighbor, goal)) {
                        ShadowCaster.CastShadowRay(field, current, goal);
                        isLosCorner = true;
                        break;
                    }
                }
            }

            if (isLosCorner) {
                currentCell.UnsetFlag(CellFlags.HasLineOfSight);
                wavefrontOutput.Add(current);
                return;
            }

            if (currentCell.HasFlag(CellFlags.WaveFrontBlocked)) {
                currentCell.UnsetFlag(CellFlags.HasLineOfSight);
                wavefrontOutput.Add(current);
                return;
            }

            foreach (var direction in Directions.Cardinal) {
                var neighbor = current + Directions.Offset(direction);

                if (!field.IsInBounds(neighbor.x, neighbor.y))
                    continue;

                if (field[neighbor.x, neighbor.y].cost > Cell.DefaultCost)
                    continue;

                var neighborCell = field[neighbor.x, neighbor.y]; // ref was used here
                if (neighborCell.HasFlag(CellFlags.HasLineOfSight))
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