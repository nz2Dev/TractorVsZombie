using System;
using System.Collections.Generic;

using UnityEngine;

public class NavigationService {

    private static readonly Vector3[] SampleOffsets = new Vector3[] {
        Vector3.zero,
        Vector3.forward,
        new Vector3(0.71f, 0, 0.71f),
        Vector3.right,
        new Vector3(0.71f, 0, -0.71f),
        Vector3.back,
        new Vector3(-0.71f, 0, -0.71f),
        Vector3.left,
        new Vector3(-0.71f, 0, 0.71f),
    };

    private readonly FlowFieldsSurface surface;

    public NavigationService(FlowFieldsSurface surface) {
        this.surface = surface;
    }

    public virtual Vector3 GetFlowVector(Vector3 worldSpacePosition) {
        return surface.GetFlowVectorClamped(worldSpacePosition);
    }

    public virtual void SetGoal(Vector3 worldSpacePosition) {
        surface.SetGoal(worldSpacePosition);
    }

    private Vector3 MultiSampleFlowVector(Vector3 worldPos) {
        var sumsCount = 0;
        Vector3 flowSum = Vector3.zero;
        foreach (var offset in SampleOffsets) {
            var samplePosition = worldPos + offset;
            var sampledFlowVector = surface.GetFlowVectorClamped(samplePosition);
            if (sampledFlowVector.magnitude > 0) {
                flowSum += sampledFlowVector;
                sumsCount++;
            }
        }
        
        if (sumsCount > 0) {
            return flowSum / sumsCount;
        } else {
            return Vector3.zero;
        }
    }
}