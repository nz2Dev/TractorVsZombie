using System;
using System.Collections.Generic;

using UnityEngine;

public struct WheelAxisPose {
    public Vector3 positionL;
    public Quaternion rotationL;
    public Vector3 positionR;
    public Quaternion rotationR;
}

public struct VehicleBodyPose {
    public Vector3 position;
    public Vector3 velocity;
    public Quaternion rotation;
}

[Serializable]
public struct VehiclePhysicsData {
    public float mass;
    public Vector3 baseSize;
    public float wheelMass;
    public float forwardFrictionStiffness;
    public float sidewayFrictionStiffness;
    public WheelAxisData[] wheelAxisDatas;
    public float towingTongueLength;
}

[Serializable]
public struct WheelAxisData {
    public float forwardOffset;
    public float upOffset;
    public float halfLength;
    public float radius;
    public bool drive;
    public bool stear;
}

public class VehicleService {
    
    private readonly VehiclePhysicsRoot physicsRoot;
    private int idCounter;
    private Dictionary<int, VehiclePhysicsRig> physicsRegistry = new ();

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
                    physicsData.wheelMass,
                    physicsData.towingTongueLength
                ); 
            } else {
                vehiclePhysics.CreateWheelAxis(
                    wheelAxis.halfLength * 2, 
                    wheelAxis.upOffset, 
                    wheelAxis.forwardOffset, 
                    wheelAxis.radius, 
                    physicsData.wheelMass,
                    wheelAxis.drive, 
                    wheelAxis.stear
                );
            }
        }

        var nextRigId = idCounter++;
        physicsRegistry[nextRigId] = vehiclePhysics;
        return nextRigId;
    }

    public void DeleteVehicle(int vehicleId) {
        var vehiclePhysics = physicsRegistry[vehicleId];
        physicsRoot.DestroyRig(vehiclePhysics);
        physicsRegistry.Remove(vehicleId);
    }

    public void UpdateBase(int vehicleIndex, float mass) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        vehiclePhysics.UpdateBase(mass);
    }

    public void UpdateWheels(int vehicleIndex, float mass, float forwardFrictionStiffness, float sidewayFrictionStiffness) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        vehiclePhysics.UpdateWheels(
            mass,
            new WheelFrictionCurve {
                asymptoteSlip = 0.4f,
                asymptoteValue = 1,
                extremumSlip = 0.8f,
                extremumValue = 0.5f,
                stiffness = forwardFrictionStiffness,
            }, 
            new WheelFrictionCurve {
                asymptoteSlip = 0.2f,
                asymptoteValue = 1,
                extremumSlip = 0.5f,
                extremumValue = 0.75f,
                stiffness = sidewayFrictionStiffness,
            });
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

    public void SetVehicleBreaks(int vehicleIndex, float breaksTorque) {
        var construction = physicsRegistry[vehicleIndex];
        for (int axisIndex = 0; axisIndex < construction.AxisCount; axisIndex++) {
            construction.SetAxisBreaksTorque(axisIndex, breaksTorque);
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
            rotation = vehiclePhysics.Rotation,
            velocity = vehiclePhysics.Velocity
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