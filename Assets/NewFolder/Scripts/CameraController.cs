using UnityEngine;

public class CameraController {
    
    private CameraService cameraService;
    private VehicleService vehicleService;

    public CameraController(CameraService cameraService, VehicleService vehicleService) {
        this.cameraService = cameraService;
        this.vehicleService = vehicleService;
    }

    public void Init() {
        cameraService.InitTopDownFollowTarget(Vector3.zero, 10f);
    }

    public void Update() {
        var driveVehiclePosition = vehicleService.GetVehiclePose(0).position;
        cameraService.UpdateTopDownFollowPosition(driveVehiclePosition);
    }
    
}