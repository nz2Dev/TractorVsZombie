using System.Collections.Generic;

using UnityEngine;

public static class FlowFieldLineOfSightPass {
    
    public static void ComputeLineOfSight(FlowField flowField, Vector2Int goal, List<Vector2Int> wavefrontOutput) {
        wavefrontOutput.Clear();
        wavefrontOutput.Add(goal);
    }

}