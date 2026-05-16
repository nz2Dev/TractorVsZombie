#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VehicleChassie))]
public class VehicleChassieEditor : Editor {

    private bool _copyFromPrototype = true;

    public override void OnInspectorGUI() {
        _copyFromPrototype = EditorGUILayout.Toggle("Copy from Wheel Prototype", _copyFromPrototype);
        EditorGUILayout.Space();
        DrawDefaultInspector();
    }

    private void OnEnable() {
        EditorApplication.update += EditorUpdate;
    }

    private void OnDisable() {
        EditorApplication.update -= EditorUpdate;
    }

    private void EditorUpdate() {
        if (!_copyFromPrototype) 
            return;

        var chassie = (VehicleChassie) target;
        CopyPrototypeIntoAxis(chassie.frontAxisConfig, chassie.frontAxis);
        CopyPrototypeIntoAxis(chassie.rearAxisConfig, chassie.rearAxis);
    }

    private static void CopyPrototypeIntoAxis(WheelAxisConfig axisConfig, WheelAxis axis) {
        var prototype = axisConfig.wheelPrototype;
        if (prototype == null) 
            return;

        if (axis.leftWheel != null && axis.leftWheel != prototype) {
            EditorUtility.CopySerialized(prototype, axis.leftWheel);
        }

        if (axis.rightWheel != null && axis.rightWheel != prototype) {
            EditorUtility.CopySerialized(prototype, axis.rightWheel);
        }
    }
}
#endif
