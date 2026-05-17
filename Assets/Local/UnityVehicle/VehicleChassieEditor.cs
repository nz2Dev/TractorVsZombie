#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VehicleChassie))]
public class VehicleChassieEditor : Editor {

    private static bool mirrorRightWheels = true;
    private static bool mirrorRearAxis = false;

    private bool prototypingFoldout = true;
    private WheelCollider wheelPrototype;
    private SerializedProperty frontAxisProperty;
    private SerializedProperty rearAxisProperty;

    public override void OnInspectorGUI() {
        var chassie = target as VehicleChassie;
        
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Mirroring", EditorStyles.boldLabel);
        mirrorRightWheels = EditorGUILayout.Toggle("Mirror Right Wheel", mirrorRightWheels);
        mirrorRearAxis = EditorGUILayout.Toggle("Mirror Rear Axis", mirrorRearAxis);
        
        EditorGUILayout.Space();
        prototypingFoldout = EditorGUILayout.Foldout(prototypingFoldout, "Prototype");
        if (prototypingFoldout) {
            wheelPrototype = (WheelCollider) EditorGUILayout.ObjectField("Wheel Prototype", wheelPrototype, typeof(WheelCollider), true);
            if (wheelPrototype == null) {
                if (GUILayout.Button("Create Default")) {
                    var defaultWheelPrototype = new GameObject("Wheel Prototype", typeof(WheelCollider));
                    defaultWheelPrototype.transform.SetParent(chassie.transform, false);
                    wheelPrototype = defaultWheelPrototype.GetComponent<WheelCollider>();
                }
                if (GUILayout.Button("Find First Nested")) {
                    var nested = chassie.GetComponentInChildren<WheelCollider>(includeInactive: true);
                    wheelPrototype = nested;
                }
            }
            if (wheelPrototype != null) {
                wheelPrototype.gameObject.SetActive(false);
                if (GUILayout.Button("Clear")) {
                    DestroyImmediate(wheelPrototype.gameObject);
                    wheelPrototype = null;
                }
            }
        }

        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(frontAxisProperty);
        if (wheelPrototype != null && (chassie.frontAxis.leftWheel == null || chassie.frontAxis.rightWheel == null)) {
            if (GUILayout.Button("Copy From Prototype")) {
                chassie.frontAxis.leftWheel = Instantiate(wheelPrototype, chassie.transform);
                chassie.frontAxis.leftWheel.gameObject.name = "Front Left Wheel (Clone)";
                chassie.frontAxis.leftWheel.gameObject.SetActive(true);
                chassie.frontAxis.rightWheel = Instantiate(wheelPrototype, chassie.transform);
                chassie.frontAxis.rightWheel.gameObject.name = "Front Right Wheel (Clone)";
                chassie.frontAxis.rightWheel.gameObject.SetActive(true);
            }
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(rearAxisProperty);
        if (wheelPrototype != null && (chassie.rearAxis.leftWheel == null || chassie.rearAxis.rightWheel == null)) {
            if (GUILayout.Button("Copy From Prototype")) {
                chassie.rearAxis.leftWheel = Instantiate(wheelPrototype, chassie.transform);
                chassie.rearAxis.leftWheel.gameObject.name = "Rear Left Wheel (Clone)";
                chassie.rearAxis.leftWheel.gameObject.SetActive(true);
                chassie.rearAxis.rightWheel = Instantiate(wheelPrototype, chassie.transform);
                chassie.rearAxis.rightWheel.gameObject.name = "Rear Right Wheel (Clone)";
                chassie.rearAxis.rightWheel.gameObject.SetActive(true);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable() {
        EditorApplication.update += EditorUpdate;
        var chassie = target as VehicleChassie;
        frontAxisProperty = serializedObject.FindProperty(nameof(chassie.frontAxis));
        rearAxisProperty = serializedObject.FindProperty(nameof(chassie.rearAxis));
    }

    private void OnDisable() {
        EditorApplication.update -= EditorUpdate;
    }

    private void EditorUpdate() {
        var chassie = (VehicleChassie) target;
        
        if (mirrorRightWheels) {
            MirrorWheelCollider(chassie.frontAxis.leftWheel, chassie.frontAxis.rightWheel);
            MirrorWheelCollider(chassie.rearAxis.leftWheel, chassie.rearAxis.rightWheel);
        }
        if (mirrorRearAxis) {
            MirrorWheelCollider(chassie.frontAxis.leftWheel, chassie.rearAxis.leftWheel);
            MirrorWheelCollider(chassie.frontAxis.rightWheel, chassie.rearAxis.rightWheel);
        }
    }

    private static void MirrorWheelCollider(WheelCollider wheelSource, WheelCollider wheelDest) {
        if (wheelSource != null && wheelDest != null) {
            EditorUtility.CopySerialized(wheelSource, wheelDest);
        }
    }
}
#endif
