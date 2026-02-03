using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ORCAEnvironment))]
public class ORCAEnvironmentEditor : Editor {

    private ORCAEnvironment environment;

    private void OnEnable() {
        environment = target as ORCAEnvironment;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Bake")) {
            environment.BakeObstacles();
        }
    }
}