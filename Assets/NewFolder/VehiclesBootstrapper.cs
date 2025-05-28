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
        vehicleSimulationService.CreateVehicle(source.baseCollider, source.wheelAxisDatas);
        
        vehicleView = new (container: transform);
        vehicleView.AddVehicle(source.baseGeometry, source.baseGeometryFit,
            source.wheelGeometry, source.wheelGeometryFit,
            source.wheelAxisDatas);
    }
    
    [ContextMenu("Preview")]
    private void Preview() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        vehicleSimulationService = new (physicsContainer: transform);
        vehicleSimulationService.CreateVehicle(source.baseCollider, source.wheelAxisDatas);
        
        vehicleView = new (container: transform);
        vehicleView.AddVehicle(source.baseGeometry, source.baseGeometryFit,
            source.wheelGeometry, source.wheelGeometryFit,
            source.wheelAxisDatas);
    }

    private void Update() {
        vehicleSimulationService.SetVehicleGasThrottle(vehicleIndex: 0, gasThrottle);

        var vehiclePose = vehicleSimulationService.GetVehiclePose(vehicleIndex: 0);
        vehicleView.UpdateVehiclePose(vehicleIndex: 0, vehiclePose);

        for (int wheelAxisIndex = 0; wheelAxisIndex < source.wheelAxisDatas.Length; wheelAxisIndex++) {
            var wheelAxisPose = vehicleSimulationService.GetVehicleWheelAxisPose(vehicleIndex:0, axisIndex: wheelAxisIndex);
            vehicleView.UpdateWheelAxisPose(vehicleIndex: 0, wheelAxisIndex, wheelAxisPose);
        }
    }

}
