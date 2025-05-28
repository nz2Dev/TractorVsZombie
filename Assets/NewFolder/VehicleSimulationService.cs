using System.Collections.Generic;

using UnityEngine;

public class VehicleSimulationService {
    
    private List<VehiclePhysics> physicsRegistry = new ();
    private Transform physicsContainer;

    public VehicleSimulationService(Transform physicsContainer) {
        this.physicsContainer = physicsContainer;
    }

    public void CreateVehicle(Collider baseColliderPrefab, WheelRowConstructionInfo[] wheels) {
        var vehiclePhysics = new VehiclePhysics(physicsContainer);
        vehiclePhysics.ConfigureBase(baseColliderPrefab);
        
        foreach (var wheelAxis in wheels)
            vehiclePhysics.CreateWheelAxis(
                wheelAxis.rowOffset * 2, 
                wheelAxis.verticalOffset, 
                wheelAxis.horizontalOffset, 
                wheelAxis.radius, 
                wheelAxis.drive, 
                wheelAxis.stear
            );

        physicsRegistry.Add(vehiclePhysics);
    }

    public void SetVehicleGasThrottle(int vehicleIndex, float v) {
        const float maxTorque = 400;
        var construction = physicsRegistry[vehicleIndex];
        for (int axisIndex = 0; axisIndex < construction.AxisCount; axisIndex++) {
            if (construction.IsDriveAxis(axisIndex)) {
                construction.SetAxisMotorTorque(axisIndex, v * maxTorque);
                construction.SetAxisBreaksTorque(axisIndex, 0);
            }            
        }
    }

    public VehiclePose GetVehiclePose(int vehicleIndex) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        return new VehiclePose {
            position = vehiclePhysics.Position,
            rotation = vehiclePhysics.Rotation
        };
    }

    public WheelAxisPose GetVehicleWheelAxisPose(int vehicleIndex, int axisIndex) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        vehiclePhysics.GetAxisPose(axisIndex, out var positionL, out var rotationL, out var positionR, out var rotationR);
        return new WheelAxisPose {
            positionL = positionL,
            rotationL = rotationL,
            positionR = positionR,
            rotationR = rotationR
        };
    }

}