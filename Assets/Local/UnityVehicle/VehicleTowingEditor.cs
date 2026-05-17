#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VehicleTowing))]
public class VehicleTowingEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var towing = target as VehicleTowing;
        if (towing.towingJoint == null) {
            if (GUILayout.Button("Create Joint")) {
                towing.towingJoint = towing.towingConnector.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
            }
        }
        
        if (towing.towingJoint.gameObject != towing.towingConnector.rigidbody.gameObject) {
            EditorGUILayout.HelpBox("towing joint is not on towing connector rigidboy", MessageType.Error);
        }
    }
}
#endif