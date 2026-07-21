using System;
using System.Collections.Generic;

using UnityEngine;

public static class FlowFieldLineOfSightPass {
    
    public static void ComputeLineOfSight(FlowField flowField, Vector2Int goal, List<Vector2Int> wavefrontOutput) {
        wavefrontOutput.Clear();
        wavefrontOutput.Add(goal);
    }

    internal static void CastShadowRay(FlowField field, Vector2Int start, Vector2Int end) {
        throw new NotImplementedException();
    }

    internal static bool IsLosCorner(FlowField field, Vector2Int cell, Vector2Int test, Vector2Int goal) {
        throw new NotImplementedException();
    }
}