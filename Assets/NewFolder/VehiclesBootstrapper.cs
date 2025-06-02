using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclesBootstrapper : MonoBehaviour {
    
    [SerializeField] private VehicleEntity driveVehicle;
    [SerializeField] private VehicleEntity trailerVehicle;
    [SerializeField] private float gasThrottle = 0;

    private List<VehicleEntity> vehicles;
    private VehicleService vehicleSimulationService;
    private VehicleView vehicleView;

    private void Start() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        vehicles = new();
        vehicleView = new (container: transform);
        vehicleSimulationService = new (physicsContainer: transform);

        vehicleSimulationService.CreateVehicle(driveVehicle.baseBounds, driveVehicle.wheelAxisDatas);
        vehicleView.AddVehicle(driveVehicle.baseGeometry, driveVehicle.wheelGeometry, driveVehicle.wheelAxisDatas);
        vehicles.Add(driveVehicle);

        vehicleSimulationService.CreateVehicle(trailerVehicle.baseBounds, trailerVehicle.wheelAxisDatas, new Vector3(0, 0, -2f));
        vehicleView.AddVehicle(trailerVehicle.baseGeometry, trailerVehicle.wheelGeometry, trailerVehicle.wheelAxisDatas);
        vehicles.Add(trailerVehicle);

        vehicleSimulationService.ConnectVehicleWithHinge(headVehicleIndex: 0, -0.7f, tailVehicleIndex: 1, 0.7f);
    }
    
    [ContextMenu("Preview")]
    private void Preview() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        vehicleSimulationService = new (physicsContainer: transform);
        vehicleSimulationService.CreateVehicle(driveVehicle.baseBounds, driveVehicle.wheelAxisDatas);
        
        vehicleView = new (container: transform);
        vehicleView.AddVehicle(driveVehicle.baseGeometry, driveVehicle.wheelGeometry, driveVehicle.wheelAxisDatas);
    }

    private void Update() {
        vehicleSimulationService.SetVehicleGasThrottle(vehicleIndex: 0, gasThrottle);

        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicleData = vehicles[vehicleIndex];
            var vehiclePose = vehicleSimulationService.GetVehiclePose(vehicleIndex);
            vehicleView.UpdateVehiclePose(vehicleIndex, vehiclePose);

            for (int wheelAxisIndex = 0; wheelAxisIndex < vehicleData.wheelAxisDatas.Length; wheelAxisIndex++) {
                var wheelAxisPose = vehicleSimulationService.GetVehicleWheelAxisPose(vehicleIndex, wheelAxisIndex);
                vehicleView.UpdateWheelAxisPose(vehicleIndex, wheelAxisIndex, wheelAxisPose);
            }    
        }
    }

}
