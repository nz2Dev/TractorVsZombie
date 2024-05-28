using UnityEngine;

public class DefaultVehicleModel : MonoBehaviour, Vehicle.IVehicleOutput, Vehicle.IVehicleInput {

    [SerializeField] private bool projectWorldUp = true;

    public Vector3 GetBasePosition() {
        return transform.position;
    }

    public Vector3 GetForwardDirection() {
        return transform.forward;
    }

    public void OnVehicleMove(Vector3 velocityFrameDelta) {
        if (projectWorldUp && velocityFrameDelta.y > float.Epsilon) {
            velocityFrameDelta = Vector3.ProjectOnPlane(velocityFrameDelta, Vector3.up);
        }
        
        var vehicleTransform = transform;
        var newPosition = vehicleTransform.position + velocityFrameDelta;
        
        vehicleTransform.LookAt(newPosition, Vector3.up);
        vehicleTransform.position = newPosition;
    }
}