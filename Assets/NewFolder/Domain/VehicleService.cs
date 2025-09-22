using System;
using System.Collections.Generic;

using UnityEngine;

public class VehicleService {
    
    private readonly VehiclePhysicsRoot physicsRoot;
    private List<VehiclePhysicsRig> physicsRegistry = new ();

    public VehicleService(VehiclePhysicsRoot physicsRoot) {
        this.physicsRoot = physicsRoot;
    }

    public int CreateVehicle(Vector3 position, VehiclePhysicsData physicsData) {
        var vehiclePhysics = physicsRoot.CreateRig(position, physicsData.mass);
        vehiclePhysics.ConfigureBase(physicsData.baseSize);
        
        var hasTowingTongue = physicsData.towingTongueLength > 0;
        for (int i = 0; i < physicsData.wheelAxisDatas.Length; i++) {
            bool isLastAxis = i == physicsData.wheelAxisDatas.Length - 1;
            var wheelAxis = physicsData.wheelAxisDatas[i];
            if (hasTowingTongue && isLastAxis) {
                vehiclePhysics.CreateTowingWheelAxis(
                    wheelAxis.halfLength * 2,
                    wheelAxis.upOffset,
                    wheelAxis.forwardOffset,
                    wheelAxis.radius,
                    physicsData.towingTongueLength
                ); 
            } else {
                vehiclePhysics.CreateWheelAxis(
                    wheelAxis.halfLength * 2, 
                    wheelAxis.upOffset, 
                    wheelAxis.forwardOffset, 
                    wheelAxis.radius, 
                    wheelAxis.drive, 
                    wheelAxis.stear
                );
            }
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

    public void SetVehicleEngineTorque(int vehicleIndex, float engineTorque) {
        const float minTorqueToEaseFriction = 0.1f;
        
        var construction = physicsRegistry[vehicleIndex];
        for (int axisIndex = 0; axisIndex < construction.AxisCount; axisIndex++) {
            var axisTorque = construction.IsDriveAxis(axisIndex) ? engineTorque : minTorqueToEaseFriction;
            construction.SetAxisMotorTorque(axisIndex, axisTorque);
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

    public VehicleBodyPose GetVehiclePose(int vehicleIndex) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        return new VehicleBodyPose {
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

    public Quaternion GetTowingTonguePose(int vehicleIndex) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        vehiclePhysics.GetTowingAxisPose(out var positionL, out var rotationL, out var positionR, out var rotationR, out var tipRotation);
        return tipRotation;
    }

}