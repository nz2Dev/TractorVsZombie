using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclesBootstrapper : MonoBehaviour {
    
    [SerializeField] private VehicleEntity driveVehicle;
    [SerializeField] private VehicleEntity trailerVehicle;
    [SerializeField] private int trailersCount = 3;

    private List<VehicleEntity> vehicles;
    private VehicleService vehicleService;
    private VehicleView vehicleView;
    private CameraService cameraService;

    private void Start() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        vehicles = new();
        vehicleView = new (container: null);
        vehicleService = new (physicsContainer: null);
        cameraService = new CameraService(Camera.main);

        var driveVehiclePosition = Vector3.zero;
        vehicleService.CreateVehicle(driveVehicle.baseSize, driveVehicle.wheelAxisDatas, driveVehicle.GetTowingWheelAxisData(), mass: driveVehicle.mass);
        vehicleView.AddVehicle(driveVehiclePosition, driveVehicle.baseGeometry, driveVehicle.wheelGeometry, driveVehicle.towingBodyGeometry, driveVehicle.wheelAxisDatas, driveVehicle.GetTowingWheelAxisData());
        vehicles.Add(driveVehicle);

        for (int i = 0; i < trailersCount; i++) {
            PlusTrailer(new Vector3(0, 0, -2f + i * -2f));
        }

        cameraService.InitTopDownFollowTarget(driveVehiclePosition, 10f);
    }

    private void PlusTrailer(Vector3 position) {
        var trailerVehiclePosition = position;
        vehicleService.CreateVehicle(trailerVehicle.baseSize, trailerVehicle.wheelAxisDatas, trailerVehicle.GetTowingWheelAxisData(), trailerVehiclePosition, mass: driveVehicle.mass);
        vehicleView.AddVehicle(trailerVehiclePosition, trailerVehicle.baseGeometry, trailerVehicle.wheelGeometry, trailerVehicle.towingBodyGeometry, trailerVehicle.wheelAxisDatas, trailerVehicle.GetTowingWheelAxisData());
        vehicles.Add(trailerVehicle);

        var lastIndex = vehicles.Count - 1;
        vehicleService.MakeTowingConnection(
            headVehicleIndex: lastIndex - 1, 
            tailVehicleIndex: lastIndex);
    }

    private void FixedUpdate() {
        vehicleService.UpdateVehicles();
    }

    private void Update() {
        const float maxSteerAngle = 35;
        var gasInput = Input.GetAxis("Vertical");
        var steerInput = Input.GetAxis("Horizontal");

        vehicleService.SetVehicleGasThrottle(vehicleIndex: 0, gasInput);
        vehicleService.SetVehicleSteer(vehicleIndex: 0, steerInput * maxSteerAngle);

        var driveVehiclePose = vehicleService.GetVehiclePose(vehicleIndex: 0);
        cameraService.UpdateTopDownFollowPosition(driveVehiclePose.position);

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

}
