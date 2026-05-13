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
    private Dictionary<int, GameObject> obstacleRegistry = new ();
    private readonly int obstacleLayer;

    public VehicleService(int obstacleLayer = 0) {
        this.obstacleLayer = obstacleLayer;
    }

    public int RegisterObstacle(Vector3 position, PhysicsObstacle prefab) {
        var id = ++idCounter;
        var go = GameObject.Instantiate(prefab.gameObject, position, Quaternion.identity);
        go.name = $"VehicleObstacle_{id}";
        go.layer = obstacleLayer;
        obstacleRegistry[id] = go;
        return id;
    }

    public void UnregisterObstacle(int id) {
        if (obstacleRegistry.TryGetValue(id, out var go)) {
            GameObject.Destroy(go);
            obstacleRegistry.Remove(id);
        }
    }

    public int CreateVehicle(Vector3 position, VehiclePhysics physicsPrefab, Quaternion rotation = default) {
        var vehiclePhysics = GameObject.Instantiate(physicsPrefab, position, rotation);
        var nextRigId = idCounter++;
        physicsRegistry[nextRigId] = vehiclePhysics;
        return nextRigId;
    }

    public void DeleteVehicle(int vehicleId) {
        var vehiclePhysics = physicsRegistry[vehicleId];
        vehiclePhysics.DestroySelf();
        physicsRegistry.Remove(vehicleId);
    }

    /// <param name="gas">0–1 throttle / negative for reverse</param>
    public void SetVehicleGas(int vehicleIndex, float gas) {
        physicsRegistry[vehicleIndex].SetGas(gas);
    }

    /// <param name="brakes">0–1</param>
    public void SetVehicleBrakes(int vehicleIndex, float brakes) {
        physicsRegistry[vehicleIndex].SetBrakes(brakes);
    }

    /// <param name="steer">-1 to 1. Traction limiting is applied inside VehiclePhysics.</param>
    public void SetVehicleSteer(int vehicleIndex, float steer) {
        physicsRegistry[vehicleIndex].SetSteer(steer);
    }

    public void SetVehicleSteerAngle(int vehicleIndex, float steerAngle) {
        physicsRegistry[vehicleIndex].SetSteerAngle(steerAngle);
    }

    public void SetVehicleInput(int vehicleIndex, float gas, float brakes, float steer) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        vehiclePhysics.SetGas(gas);
        vehiclePhysics.SetBrakes(brakes);
        vehiclePhysics.SetSteer(steer);
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