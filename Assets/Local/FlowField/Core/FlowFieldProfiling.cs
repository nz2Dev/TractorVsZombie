using Unity.Profiling;

public static class FlowFieldProfiling {
    public static ProfilerMarker FlowFieldMarker = new ProfilerMarker("FlowField");
    public static ProfilerMarker ClearFieldMarker = new ProfilerMarker("FlowField.ClearField");
    public static ProfilerMarker LineOfSightMarker = new ProfilerMarker("FlowField.LineOfSight");
    public static ProfilerMarker CostIntegrationMarker = new ProfilerMarker("FlowField.CostIntegration");
    public static ProfilerMarker VectorBuildingMarker = new ProfilerMarker("FlowField.VectorBuilding");
    public static ProfilerMarker CornerDetectionMarker = new ProfilerMarker("FlowField.CornerDetection");
    public static ProfilerMarker ShadowCastingMarker = new ProfilerMarker("FlowField.ShadowCasting");
}