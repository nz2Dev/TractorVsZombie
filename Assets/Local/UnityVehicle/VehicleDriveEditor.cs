using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VehicleDrive))]
public class VehicleDriveEditor : Editor {

    private float gasInput = 0;
    private float steerInput = 0;

    public override void OnInspectorGUI() {
        var drive = target as VehicleDrive;
        base.OnInspectorGUI();
        if (!Application.isPlaying)
            return;
        
        EditorGUILayout.Space();
        GUILayout.Label("quick input", EditorStyles.boldLabel);
        gasInput = EditorGUILayout.Slider("gas input", gasInput, 0f, 1f);
        steerInput = EditorGUILayout.Slider("steer input", steerInput, -1f, 1f);
        drive.SetGas(gasInput);
        drive.SetSteer(steerInput);
    }
}