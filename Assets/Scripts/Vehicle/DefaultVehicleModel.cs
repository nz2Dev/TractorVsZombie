using UnityEngine;

public class DefaultVehicleModel : MonoBehaviour, Vehicle.IVehicleOutput, Vehicle.IVehicleInput {

    public Vector3 GetBasePosition() {
        return transform.position;
    }

    public Vector3 GetForwardDirection() {
        return transform.forward;
    }

    public void OnVehicleMove(Vector3 velocityFrameDelta) {
        var vehicleTransform = transform;
        var newPosition = vehicleTransform.position + velocityFrameDelta;
        vehicleTransform.LookAt(newPosition, Vector3.up);
        vehicleTransform.position = newPosition;
    }
}