using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VehicleSteeringWheel))]
public class VehicleSteeringWheelEditor : Editor {
    private float steerInput = 0;

    public override void OnInspectorGUI() {
        var steering = target as VehicleSteeringWheel;
        base.OnInspectorGUI();
        if (!Application.isPlaying)
            return;
        
        EditorGUILayout.Space();
        GUILayout.Label("quick input", EditorStyles.boldLabel);
        steerInput = EditorGUILayout.Slider("steer input", steerInput, -1f, 1f);
        steering.SetSteer(steerInput);
    }
}