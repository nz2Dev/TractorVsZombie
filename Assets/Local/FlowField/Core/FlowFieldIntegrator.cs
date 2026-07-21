using System.Collections.Generic;

using UnityEngine;

public static partial class FlowFieldIntegrator {

    private static readonly List<Vector2Int> wavefrontBuffer = new();

    public static void Integrate(FlowField flowField, Vector2Int goal) {
        wavefrontBuffer.Clear();
        LineOfSightPass.ComputeLineOfSight(flowField, goal, wavefrontBuffer);
        CostIntegrationPass.ComputeCosts(flowField, goal, wavefrontBuffer);
        VectorBuilderPass.ComputeFlow(flowField, goal);
    }

}