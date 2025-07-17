using System;
using System.Collections.Generic;

using UnityEngine;

public class NavigationService {

    private readonly FlowFieldsSurface surface;

    public NavigationService(FlowFieldsSurface surface) {
        this.surface = surface;
    }

    public Vector3 GetFlowVector(Vector3 worldSpacePosition) {
        return surface.GetFlowVector(worldSpacePosition);
    }

    public void SetGoal(Vector3 worldSpacePosition) {
        surface.SetGoal(worldSpacePosition);
    }
}