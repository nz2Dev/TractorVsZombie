using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclesBootstrapper : MonoBehaviour {
    
    [SerializeField] private VehicleEntity driveVehicle;
    [SerializeField] private VehicleEntity trailerVehicle;

    private List<VehicleEntity> vehicles;
    private VehicleService vehicleService;
    private VehicleView vehicleView;

    private void Start() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        vehicles = new();
        vehicleView = new (container: transform);
        vehicleService = new (physicsContainer: transform);

        var driveVehiclePosition = Vector3.zero;
        vehicleService.CreateVehicle(driveVehicle.baseSize, driveVehicle.wheelAxisDatas);
        vehicleView.AddVehicle(driveVehiclePosition, driveVehicle.baseGeometry, driveVehicle.wheelGeometry, driveVehicle.wheelAxisDatas);
        vehicles.Add(driveVehicle);

        var trailerVehiclePosition = new Vector3(0, 0, -2f);
        vehicleService.CreateVehicle(trailerVehicle.baseSize, trailerVehicle.wheelAxisDatas, trailerVehiclePosition);
        vehicleView.AddVehicle(trailerVehiclePosition, trailerVehicle.baseGeometry, trailerVehicle.wheelGeometry, trailerVehicle.wheelAxisDatas);
        vehicles.Add(trailerVehicle);

        vehicleService.ConnectVehicleWithHinge(headVehicleIndex: 0, -0.7f, tailVehicleIndex: 1, 0.7f);
    }
    
    [ContextMenu("Preview")]
    private void Preview() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        vehicleService = new (physicsContainer: transform);
        vehicleService.CreateVehicle(driveVehicle.baseSize, driveVehicle.wheelAxisDatas);
        
        vehicleView = new (container: transform);
        vehicleView.AddVehicle(Vector3.zero, driveVehicle.baseGeometry, driveVehicle.wheelGeometry, driveVehicle.wheelAxisDatas);
    }

    private void Update() {
        const float maxSteerAngle = 45;
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
        }
    }

}
