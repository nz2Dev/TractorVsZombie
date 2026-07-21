using System.Collections.Generic;

using UnityEngine;

public static class FlowFieldIntegrator {

    private static readonly List<Vector2Int> wavefrontBuffer = new();

    public static void Integrate(FlowField flowField, Vector2Int goal) {
        wavefrontBuffer.Clear();
        FlowFieldLineOfSightPass.ComputeLineOfSight(flowField, goal, wavefrontBuffer);
        FlowFieldCostIntegrationPass.ComputeCosts(flowField, goal, wavefrontBuffer);
        FlowFieldVectorBuilderPass.ComputeFlow(flowField, goal);
    }

}