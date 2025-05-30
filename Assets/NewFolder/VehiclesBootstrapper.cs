using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclesBootstrapper : MonoBehaviour {
    
    [SerializeField] private VehicleEntity source;
    [SerializeField] private float gasThrottle = 0;

    private VehicleSimulationService vehicleSimulationService;
    private VehicleView vehicleView;

    private void Start() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        vehicleSimulationService = new (physicsContainer: transform);
        vehicleSimulationService.CreateVehicle(source.baseBounds, source.wheelAxisDatas);
        vehicleSimulationService.CreateVehicle(source.baseBounds, source.wheelAxisDatas, new Vector3(0, 0, -2f));
        vehicleSimulationService.ConnectVehicleWithHinge(headVehicleIndex: 0, -0.7f, tailVehicleIndex: 1, 0.7f);
        
        vehicleView = new (container: transform);
        vehicleView.AddVehicle(source.baseGeometry, source.baseGeometryFit,
            source.wheelGeometry, source.wheelGeometryFit,
            source.wheelAxisDatas);
        vehicleView.AddVehicle(source.baseGeometry, source.baseGeometryFit,
            source.wheelGeometry, source.wheelGeometryFit,
            source.wheelAxisDatas);
    }
    
    [ContextMenu("Preview")]
    private void Preview() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        vehicleSimulationService = new (physicsContainer: transform);
        vehicleSimulationService.CreateVehicle(source.baseBounds, source.wheelAxisDatas);
        
        vehicleView = new (container: transform);
        vehicleView.AddVehicle(source.baseGeometry, source.baseGeometryFit,
            source.wheelGeometry, source.wheelGeometryFit,
            source.wheelAxisDatas);
    }

    private void Update() {
        vehicleSimulationService.SetVehicleGasThrottle(vehicleIndex: 0, gasThrottle);

        const int vehicleCount = 2;
        for (int vehicleIndex = 0; vehicleIndex < vehicleCount; vehicleIndex++) {
            var vehiclePose = vehicleSimulationService.GetVehiclePose(vehicleIndex);
            vehicleView.UpdateVehiclePose(vehicleIndex, vehiclePose);

            for (int wheelAxisIndex = 0; wheelAxisIndex < source.wheelAxisDatas.Length; wheelAxisIndex++) {
                var wheelAxisPose = vehicleSimulationService.GetVehicleWheelAxisPose(vehicleIndex, wheelAxisIndex);
                vehicleView.UpdateWheelAxisPose(vehicleIndex, wheelAxisIndex, wheelAxisPose);
            }    
        }
    }

}
