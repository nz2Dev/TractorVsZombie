#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices.WindowsRuntime;

[CustomEditor(typeof(VehicleTowing))]
public class VehicleTowingEditor : Editor {

    private UnityVehicle testPullingVehicle;

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

        if (Application.isPlaying) {
            testPullingVehicle = (UnityVehicle) EditorGUILayout.ObjectField("test pulling vehicle", testPullingVehicle, typeof(UnityVehicle), true);
            if (testPullingVehicle != null && GUILayout.Button("Active Connection")) {
                towing.MakeConnection(testPullingVehicle.Chassie);
            }
        }
    }

}
#endif