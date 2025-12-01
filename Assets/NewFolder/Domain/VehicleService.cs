using System;
using System.Collections.Generic;

using UnityEngine;

public struct WheelAxisPose {
    public Vector3 positionL;
    public Quaternion rotationL;
    public Vector3 positionR;
    public Quaternion rotationR;
}

public struct VehicleState {
    public Vector3 velocity;
    public Vector3 position;
    public Quaternion rotation;
    public WheelAxisPose frontAxis;
    public WheelAxisPose rearAxis;
}

public class VehicleService {
    
    private int idCounter;
    private Dictionary<int, VehiclePhysics> physicsRegistry = new ();

    public VehicleService() {
    }

    public int CreateVehicle(Vector3 position, VehiclePhysics physicsPrefab) {
        var vehiclePhysics = GameObject.Instantiate(physicsPrefab, position, Quaternion.identity);
        var nextRigId = idCounter++;
        physicsRegistry[nextRigId] = vehiclePhysics;
        return nextRigId;
    }

    public void DeleteVehicle(int vehicleId) {
        var vehiclePhysics = physicsRegistry[vehicleId];
        GameObject.Destroy(vehiclePhysics.gameObject);
        physicsRegistry.Remove(vehicleId);
    }

    public void SetVehicleSteer(int vehicleIndex, float steerDegrees) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        vehiclePhysics.SetSteerAngle(steerDegrees);
    }

    public void SetVehicleEngineTorque(int vehicleIndex, float engineTorque) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        vehiclePhysics.SetMotorTorque(engineTorque);
    }

    public void SetVehicleBreaks(int vehicleIndex, float breaksTorque) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        vehiclePhysics.SetBreaksTorque(breaksTorque);
    }

    public void UpdateVehiclePose(int vehicleIndex, Vector3 position, Quaternion rotation) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        vehiclePhysics.Transform(position, rotation);
    }

    public void MakeTowingConnection(int headVehicleIndex, int tailVehicleIndex) {
        var headRig = physicsRegistry[headVehicleIndex];
        var tailRig = physicsRegistry[tailVehicleIndex];
        tailRig.SetPullingVehicle(headRig);
        tailRig.MakeLooseTowingConnection();
        tailRig.CollapseTowingConnection();
    }

    public void ClearTowingConnection(int vehicleIndex) {
        var physics = physicsRegistry[vehicleIndex];
        physics.ClearTowingConnection();
    }

    public VehicleState GetVehicleState(int vehicleId) {
        var vehiclePhysics = physicsRegistry[vehicleId];
        vehiclePhysics.FrontAxis.GetLeftWheelPose(out var fLeftPos, out var fLeftRot);
        vehiclePhysics.FrontAxis.GetRightWheelPose(out var fRightPos, out var fRightRot);
        vehiclePhysics.RearAxis.GetLeftWheelPose(out var rLeftPos, out var rLeftRot);
        vehiclePhysics.RearAxis.GetRightWheelPose(out var rRightPos, out var rRightRot);
        return new VehicleState {
            position = vehiclePhysics.Position,
            rotation = vehiclePhysics.Rotation,
            velocity = vehiclePhysics.Velocity,
            frontAxis = new WheelAxisPose {
                positionL = fLeftPos, 
                rotationL = fLeftRot,
                positionR = fRightPos, 
                rotationR = fRightRot
            },
            rearAxis = new WheelAxisPose {
                positionL = rLeftPos, 
                rotationL = rLeftRot,
                positionR = rRightPos, 
                rotationR = rRightRot
            }
        };
    }

}