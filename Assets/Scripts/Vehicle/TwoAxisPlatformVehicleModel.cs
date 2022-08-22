using UnityEngine;

public class TwoAxisPlatformVehicleModel : MonoBehaviour, Vehicle.IVehicleInput, Vehicle.IVehicleOutput {

    private TwoAxisMovePlatform _twoAxisPlatform;

    public Transform BaseTransform => _twoAxisPlatform.TurnAxis.transform;

    private void Awake() {
        _twoAxisPlatform = GetComponent<TwoAxisMovePlatform>();
    }

    private void Start() {
        var vehicle = GetComponent<Vehicle>();
        vehicle.SetVehicleInput(this);
        vehicle.SetVehicleOutput(this);
    }

    Vector3 Vehicle.IVehicleInput.GetBasePosition() {
        return _twoAxisPlatform.TurnAxis.position;
    }

    Vector3 Vehicle.IVehicleInput.GetForwardDirection() {
        return _twoAxisPlatform.TurnAxis.forward;
    }

    void Vehicle.IVehicleOutput.OnVehicleMove(Vector3 velocityFrameDelta) {
        var outputTransform = _twoAxisPlatform.TurnAxis;
        var newPosition = outputTransform.position + velocityFrameDelta;
        outputTransform.LookAt(newPosition, Vector3.up);
        outputTransform.position = newPosition;
    }
}