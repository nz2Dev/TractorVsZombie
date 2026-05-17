#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnityVehicle))]
public class UnityVehicleEditor : Editor {

    private static bool showRigidbodyComponent = false;

    public override void OnInspectorGUI() {
        var vehicle = target as UnityVehicle;
        showRigidbodyComponent = EditorGUILayout.Toggle("Show rigidbody component", showRigidbodyComponent);
        var rigidbody = vehicle.GetComponent<Rigidbody>();
        if (showRigidbodyComponent) {
            rigidbody.hideFlags = HideFlags.None;
        } else if (rigidbody.hideFlags != HideFlags.HideInInspector) {
            rigidbody.hideFlags = HideFlags.HideInInspector;
        }
        base.OnInspectorGUI();
    }
}
#endif