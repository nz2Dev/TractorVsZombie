using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VehicleSteeringAxle))]
public class VehicleSteeringAxleEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var steeringAxle = target as VehicleSteeringAxle;
        if (steeringAxle.axleRigidbody == null) {
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