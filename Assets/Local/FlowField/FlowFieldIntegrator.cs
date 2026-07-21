using System.Collections.Generic;

using UnityEngine;

public static class FlowFieldIntegrator {

    public static void Integrate(FlowField flowField, Vector2Int goal) {
        FlowFieldCostIntegrationPass.ComputeCosts(flowField, goal);
        FlowFieldVectorBuilderPass.ComputeFlow(flowField, goal);
    }

}