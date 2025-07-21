using System.Collections.Generic;

using UnityEngine;

public class VehiclesController : MonoBehaviour {
    
    private readonly int trailersCount;
    private readonly VehicleEntity driveVehicle;
    private readonly VehicleEntity trailerVehicle;

    private readonly VehicleService vehicleService;
    private readonly VehicleView vehicleView;
    private readonly List<VehicleEntity> vehicles;

    public VehiclesController(VehicleService vehicleService, VehicleView vehicleView, VehicleEntity driveVehicle, VehicleEntity trailerVehicle, int trailersCount) {
        this.vehicleService = vehicleService;
        this.vehicleView = vehicleView;
        this.driveVehicle = driveVehicle;
        this.trailerVehicle = trailerVehicle;
        this.trailersCount = trailersCount;
        this.vehicles = new();
    }

    public void Init() {
        SpawnDriveVehicle();

        for (int i = 0; i < trailersCount; i++) {
            SpawnTrailerVehicle(new Vector3(0, 0, -2f + i * -2f));
        }
    }

    public void FixedUpdate() {
        vehicleService.UpdateVehicles();
    }

    public void Update() {
        const float maxSteerAngle = 35;
        var gasInput = Input.GetAxis("Vertical");
        var steerInput = Input.GetAxis("Horizontal");

        vehicleService.SetVehicleGasThrottle(vehicleIndex: 0, gasInput);
        vehicleService.SetVehicleSteer(vehicleIndex: 0, steerInput * maxSteerAngle);

        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicleData = vehicles[vehicleIndex];
            var vehiclePose = vehicleService.GetVehiclePose(vehicleIndex);
            vehicleView.UpdateVehiclePose(vehicleIndex, vehiclePose);

            for (int wheelAxisIndex = 0; wheelAxisIndex < vehicleData.wheelAxisDatas.Length; wheelAxisIndex++) {
                var wheelAxisPose = vehicleService.GetVehicleWheelAxisPose(vehicleIndex, wheelAxisIndex);
                vehicleView.UpdateWheelAxisPose(vehicleIndex, wheelAxisIndex, wheelAxisPose);
            }   

            if (vehicleData.towingWheel) {
                var towingWheelAxisPose = vehicleService.GetVehicleTowingWheelAxisPose(vehicleIndex);
                vehicleView.UpdateTowingWheelAxisPose(vehicleIndex, towingWheelAxisPose);
            }
        }
    }

    private void SpawnDriveVehicle() {
        var driveVehiclePosition = Vector3.zero;
        vehicleService.CreateVehicle(driveVehicle.baseSize, driveVehicle.wheelAxisDatas, driveVehicle.GetTowingWheelAxisData(), mass: driveVehicle.mass);
        vehicleView.AddVehicle(driveVehiclePosition, driveVehicle.baseGeometry, driveVehicle.wheelGeometry, driveVehicle.towingBodyGeometry, driveVehicle.wheelAxisDatas, driveVehicle.GetTowingWheelAxisData());
        vehicles.Add(driveVehicle);
    }

    private void SpawnTrailerVehicle(Vector3 position) {
        var trailerVehiclePosition = position;
        vehicleService.CreateVehicle(trailerVehicle.baseSize, trailerVehicle.wheelAxisDatas, trailerVehicle.GetTowingWheelAxisData(), trailerVehiclePosition, mass: driveVehicle.mass);
        vehicleView.AddVehicle(trailerVehiclePosition, trailerVehicle.baseGeometry, trailerVehicle.wheelGeometry, trailerVehicle.towingBodyGeometry, trailerVehicle.wheelAxisDatas, trailerVehicle.GetTowingWheelAxisData());
        vehicles.Add(trailerVehicle);

        var lastIndex = vehicles.Count - 1;
        vehicleService.MakeTowingConnection(
            headVehicleIndex: lastIndex - 1, 
            tailVehicleIndex: lastIndex);
    }
}