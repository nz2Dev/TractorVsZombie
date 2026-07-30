using System.Collections.Generic;

using Unity.Profiling;

using UnityEngine;

public static partial class FlowFieldIntegrator {
    
    private static readonly List<Vector2Int> wavefrontBuffer = new();

    public static void Integrate(FlowField flowField, Vector2Int goal, bool lineOfSightPass = false) {
        using (FlowFieldProfiling.FlowFieldMarker.Auto()) {
            ClearField(flowField);
            wavefrontBuffer.Clear();
            
            if (lineOfSightPass) {
                LineOfSightPass.ComputeLineOfSight(flowField, goal, wavefrontBuffer);
            } else {
                wavefrontBuffer.Add(goal);
            }
            CostIntegrationPass.ComputeCosts(flowField, goal, wavefrontBuffer);
            VectorBuilderPass.ComputeFlow(flowField, goal);
        }
    }

    private static void ClearField(FlowField field) {
        using (FlowFieldProfiling.ClearFieldMarker.Auto()) {
            for (int x = 0; x < field.Size; x++) {
                for (int y = 0; y < field.Size; y++) {
                    ref var cell = ref field.GetRef(x, y);
                    cell.integratedCost = 0;
                    cell.flags = CellFlags.None;
                    cell.flowVector = Vector2Int.zero;
                }
            }
        }
    }

}