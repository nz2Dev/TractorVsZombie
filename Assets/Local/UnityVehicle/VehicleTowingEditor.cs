#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices.WindowsRuntime;

[CustomEditor(typeof(VehicleTowing))]
public class VehicleTowingEditor : Editor, IVehiclePullingConnectorProvider {

    private UnityVehicle testPullingVehicle;

    public VehiclePullingConnector GetPullingConnector() {
        return new VehiclePullingConnector {
            rigidbody = testPullingVehicle.GetComponent<Rigidbody>(),
            anchorOffsetLocalSpace = new Vector3(0, 0, -2f)
        };
    }

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
                towing.MakeConnection(this);
            }
        }
    }

}
#endif