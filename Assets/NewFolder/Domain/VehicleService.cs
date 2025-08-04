using System;
using System.Collections.Generic;

using UnityEngine;

public class VehicleService {
    
    private readonly VehiclePhysicsRoot physicsRoot;
    private List<VehiclePhysicsRig> physicsRegistry = new ();

    public VehicleService(VehiclePhysicsRoot physicsRoot) {
        this.physicsRoot = physicsRoot;
    }

    public int CreateVehicle(Vector3 baseSize, WheelAxisData[] wheels, TowingWheelAxisData? towingWheel = null, Vector3 position = default, float mass = 100) {
        var vehiclePhysics = physicsRoot.CreateRig(position, mass);
        vehiclePhysics.ConfigureBase(baseSize);
        
        foreach (var wheelAxis in wheels)
            vehiclePhysics.CreateWheelAxis(
                wheelAxis.halfLength * 2, 
                wheelAxis.upOffset, 
                wheelAxis.forwardOffset, 
                wheelAxis.radius, 
                wheelAxis.drive, 
                wheelAxis.stear
            );
        
        if (towingWheel.HasValue) {
            vehiclePhysics.CreateTowingWheelAxis(
                towingWheel.Value.halfLength * 2,
                towingWheel.Value.upOffset,
                towingWheel.Value.forwardOffset,
                towingWheel.Value.radius,
                towingWheel.Value.towingBodyLength
            );
        }

        physicsRegistry.Add(vehiclePhysics);
        var lastVehicleIndex = physicsRegistry.Count - 1;
        return lastVehicleIndex;
    }

    public void SetVehicleSteer(int vehicleIndex, float steerDegrees) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        for (int axisIndex = 0; axisIndex < vehiclePhysics.AxisCount; axisIndex++) {
            var steerAngle = vehiclePhysics.IsSteerAxis(axisIndex) ? steerDegrees : 0;
            vehiclePhysics.SetAxisSteerAngle(axisIndex, steerAngle);
        }
    }

    public void SetVehicleGasThrottle(int vehicleIndex, float v) {
        const float maxTorque = 1000;
        const float minTorqueToEaseFriction = 0.1f;
        
        var engineTorque = v * maxTorque;
        var construction = physicsRegistry[vehicleIndex];
        for (int axisIndex = 0; axisIndex < construction.AxisCount; axisIndex++) {
            var torque = construction.IsDriveAxis(axisIndex) ? engineTorque : minTorqueToEaseFriction;
            construction.SetAxisMotorTorque(axisIndex, torque);
        }
    }

    public void SetVehicleBreaks(int vehicleIndex, float v) {
        const float maxBreaksTorque = 400;
        var construction = physicsRegistry[vehicleIndex];
        for (int axisIndex = 0; axisIndex < construction.AxisCount; axisIndex++) {
            construction.SetAxisBreaksTorque(axisIndex, v * maxBreaksTorque);
        }
    }

    public void MakeTowingConnection(int headVehicleIndex, int tailVehicleIndex, float anchorsOffset = 0) {
        var headRig = physicsRegistry[headVehicleIndex];
        var tailRig = physicsRegistry[tailVehicleIndex];
        physicsRoot.MakeTowingConnection(headRig, tailRig, anchorsOffset);
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

    public TowingWheelAxisPose GetVehicleTowingWheelAxisPose(int vehicleIndex) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        vehiclePhysics.GetTowingAxisPose(out var positionL, out var rotationL, out var positionR, out var rotationR, out var tipRotation);
        return new TowingWheelAxisPose {
            positionL = positionL,
            rotationL = rotationL,
            positionR = positionR,
            rotationR = rotationR,
            tipRotation = tipRotation
        };
    }

}