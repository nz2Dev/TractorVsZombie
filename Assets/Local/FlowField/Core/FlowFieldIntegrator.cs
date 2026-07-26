using System.Collections.Generic;

using UnityEngine;

public static partial class FlowFieldIntegrator {

    internal static bool losEnabled = false;
    
    private static readonly List<Vector2Int> wavefrontBuffer = new();

    public static void Integrate(FlowField flowField, Vector2Int goal) {
        ClearField(flowField);
        wavefrontBuffer.Clear();
        if (losEnabled) {
            LineOfSightPass.ComputeLineOfSight(flowField, goal, wavefrontBuffer);
        } else {
            wavefrontBuffer.Add(goal);
        }
        CostIntegrationPass.ComputeCosts(flowField, goal, wavefrontBuffer);
        VectorBuilderPass.ComputeFlow(flowField, goal);
    }

    private static void ClearField(FlowField field) {
        for (int x = 0; x < field.Size; x++) {
            for (int y = 0; y < field.Size; y++) {
                field[x, y].integratedCost = 0;
                field[x, y].flags = CellFlags.None;
                field[x, y].flowVector = Vector2Int.zero;
            }
        }
    }

}