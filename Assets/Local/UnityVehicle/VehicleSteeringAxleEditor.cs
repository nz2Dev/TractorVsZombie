using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(VehicleSteeringAxle))]
public class VehicleSteeringAxleEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var steeringAxle = target as VehicleSteeringAxle;
        if (steeringAxle.axleRigidbody == null || steeringAxle.axleBoxCollider == null || steeringAxle.axleBodyJoint == null) {
            GUILayout.Space(12);
            EditorGUILayout.HelpBox("All Axle Objects Required", MessageType.Warning);

            if (GUILayout.Button("Create Default")) {
                var turningBodyObject = new GameObject("Turning Body (New)", typeof(Rigidbody), typeof(BoxCollider), typeof(ConfigurableJoint));
                turningBodyObject.transform.SetParent(steeringAxle.transform, false);
                steeringAxle.axleRigidbody = turningBodyObject.GetComponent<Rigidbody>();
                steeringAxle.axleBoxCollider = turningBodyObject.GetComponent<BoxCollider>();
                steeringAxle.axleBodyJoint = turningBodyObject.GetComponent<ConfigurableJoint>();
            }
        }
    }
}