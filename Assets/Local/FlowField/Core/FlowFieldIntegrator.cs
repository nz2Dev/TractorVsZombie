using System.Collections.Generic;

using UnityEngine;

public static partial class FlowFieldIntegrator {

    private static readonly bool losEnabled = false;
    
    private static readonly List<Vector2Int> wavefrontBuffer = new();

    public static void Integrate(FlowField flowField, Vector2Int goal) {
        ClearIntegratedCosts(flowField);
        wavefrontBuffer.Clear();
        if (losEnabled) {
            LineOfSightPass.ComputeLineOfSight(flowField, goal, wavefrontBuffer);
        } else {
            wavefrontBuffer.Add(goal);
        }
        CostIntegrationPass.ComputeCosts(flowField, goal, wavefrontBuffer);
        VectorBuilderPass.ComputeFlow(flowField, goal);
    }

    private static void ClearIntegratedCosts(FlowField field) {
        for (int x = 0; x < field.Size; x++) {
            for (int y = 0; y < field.Size; y++) {
                field[x, y].integratedCost = 0;
            }
        }
    }

}