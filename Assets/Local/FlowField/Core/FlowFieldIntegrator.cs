using System.Collections.Generic;

using UnityEngine;

public static partial class FlowFieldIntegrator {

    private static readonly bool losEnabled = false;
    
    private static readonly List<Vector2Int> wavefrontBuffer = new();

    public static void Integrate(FlowField flowField, Vector2Int goal) {
        wavefrontBuffer.Clear();
        if (losEnabled) {
            LineOfSightPass.ComputeLineOfSight(flowField, goal, wavefrontBuffer);
        } else {
            wavefrontBuffer.Add(goal);
        }
        CostIntegrationPass.ComputeCosts(flowField, goal, wavefrontBuffer);
        VectorBuilderPass.ComputeFlow(flowField, goal);
    }

}