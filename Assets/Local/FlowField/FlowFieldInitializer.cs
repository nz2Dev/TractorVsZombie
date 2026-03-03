using UnityEngine;

public static class FlowFieldInitializer {
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Initialize() {
        var runnerGO = new GameObject("FlowField Runner");
        runnerGO.AddComponent<FlowFieldRunner>();
    }

}