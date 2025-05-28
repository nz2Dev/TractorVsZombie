using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclesBootstrapper : MonoBehaviour {
    
    [SerializeField] private VehicleEntity source;
    [SerializeField] private float gasThrottle = 0;

    private VehicleSimulationService vehicleSimulationService;
    private VehicleVisuals vehicleVisuals;

    private void Start() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        vehicleSimulationService = new (physicsContainer: transform);
        vehicleSimulationService.CreateVehicle(source.baseCollider, source.wheelAxisDatas);
        
        vehicleVisuals = new VehicleVisuals(source);
        vehicleVisuals.Construct(container: transform);
    }
    
    [ContextMenu("Preview")]
    private void Preview() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        vehicleSimulationService = new (physicsContainer: transform);
        vehicleSimulationService.CreateVehicle(source.baseCollider, source.wheelAxisDatas);
        
        vehicleVisuals = new VehicleVisuals(source);
        vehicleVisuals.Construct(container: transform);
    }

    private void Update() {
        vehicleSimulationService.SetVehicleGasThrottle(vehicleIndex: 0, gasThrottle);

        var vehiclePose = vehicleSimulationService.GetVehiclePose(vehicleIndex: 0);
        vehicleVisuals.SetPositionAndRotation(vehiclePose.position, vehiclePose.rotation);

        for (int wheelRowIndex = 0; wheelRowIndex < source.wheelAxisDatas.Length; wheelRowIndex++) {
            var wheelAxisPose = vehicleSimulationService.GetVehicleWheelAxisPose(vehicleIndex:0, axisIndex: wheelRowIndex);
            vehicleVisuals.SetWheelRow(wheelRowIndex, 
                wheelAxisPose.positionL, 
                wheelAxisPose.rotationL, 
                wheelAxisPose.positionR, 
                wheelAxisPose.rotationR);
        }
    }

    

}
