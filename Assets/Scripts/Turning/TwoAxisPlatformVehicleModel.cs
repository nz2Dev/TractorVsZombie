using UnityEngine;

public class TwoAxisPlatformVehicleModel : MonoBehaviour, Vehicle.IVehicleInput, Vehicle.IVehicleOutput {

    private TwoAxisMovePlatform _twoAxisPlatform;

    private void Awake() {
        _twoAxisPlatform = GetComponent<TwoAxisMovePlatform>();
    }

    Vector3 Vehicle.IVehicleInput.GetBasePosition() {
        return _twoAxisPlatform.FrontPosition;
    }

    Vector3 Vehicle.IVehicleInput.GetForwardDirection() {
        return _twoAxisPlatform.FrontForward;
    }

    void Vehicle.IVehicleOutput.OnVehicleMove(Vector3 velocityFrameDelta) {
        _twoAxisPlatform.MoveBy(velocityFrameDelta);
    }
}