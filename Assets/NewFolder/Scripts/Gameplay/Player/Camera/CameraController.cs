using UnityEngine;

public class CameraController {
    
    private readonly CameraView view;
    
    private Vector3 vehiclePosition;

    public CameraController(CameraView view) {
        this.view = view;
    }

    public void SetVehiclePosition(Vector3 position) {
        vehiclePosition = position;
    }

    public void Update() {
        view.UpdateFollowCamera(vehiclePosition);
    }

}