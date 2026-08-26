using UnityEditor;

using UnityEngine;
using UnityEngine.Assertions;

public static class ORCAInitializer {

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void RuntimeInitialize() {
        var gameObject = new GameObject("ORCA Runner");
        gameObject.AddComponent<ORCARunner>();
    }

}