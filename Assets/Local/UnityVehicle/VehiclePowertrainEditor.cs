using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VehiclePowertrain))]
public class VehiclePowertrainEditor : Editor {

    private float gasInput = 0;

    public override void OnInspectorGUI() {
        var powertrain = target as VehiclePowertrain;
        base.OnInspectorGUI();
        if (!Application.isPlaying)
            return;
        
        EditorGUILayout.Space();
        GUILayout.Label("quick input", EditorStyles.boldLabel);
        gasInput = EditorGUILayout.Slider("gas input", gasInput, 0f, 1f);
        powertrain.SetGas(gasInput);
    }
}