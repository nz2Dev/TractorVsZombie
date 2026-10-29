using UnityEngine;

[RequireComponent(typeof(VehicleWheelRig))]
public class PlatformVisuals : MonoBehaviour {
    private VehicleWheelRig wheelRig;

    private void Awake() {
        wheelRig = GetComponent<VehicleWheelRig>();
    }

    public void SetPositionAndRotation(Vector3 pos, Quaternion rot) {
        transform.SetPositionAndRotation(pos, rot);
    }

    public void SetFrontAxis(WheelAxisPose axisPose) {
        wheelRig.SetFrontAxis(axisPose);
    }
    
    public void SetRearAxis(WheelAxisPose axisPose) {
        wheelRig.SetRearAxis(axisPose);
    }

}
