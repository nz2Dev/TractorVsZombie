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
    private Dictionary<int, UnityVehicle> vehiclesRegistry = new ();
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

    public int CreateVehicle(Vector3 position, UnityVehicle vehiclePrefab, Quaternion rotation = default) {
        var vehicle = GameObject.Instantiate(vehiclePrefab, position, rotation);
        var nextId = idCounter++;
        vehiclesRegistry[nextId] = vehicle;
        return nextId;
    }

    public void DeleteVehicle(int vehicleId) {
        var vehiclePhysics = vehiclesRegistry[vehicleId];
        vehiclePhysics.DestroySelf();
        vehiclesRegistry.Remove(vehicleId);
    }

    public void SetVehicleSteerAngle(int vehicleIndex, float steerAngle) {
        vehiclesRegistry[vehicleIndex].SteeringWheel.SetSteerAngle(steerAngle);
    }

    public void SetVehiclePowertrain(int vehicleIndex, float powerInput, float brakesInput) {
        var vehicle = vehiclesRegistry[vehicleIndex];
        var powertrain = vehicle.Powertrain;
        powertrain.SetGas(powerInput);
        powertrain.SetBrakes(brakesInput);
    }

    public void SetVehicleInput(int vehicleIndex, float gas, float brakes, float steer) {
        var vehicle = vehiclesRegistry[vehicleIndex];
        var powertrain = vehicle.Powertrain;
        powertrain.SetGas(gas);
        powertrain.SetBrakes(brakes);
        var steeringWheel = vehicle.SteeringWheel;
        steeringWheel.SetSteer(steer);
    }

    public void ApplyDragForce(int vehicleIndex, float force, ForceMode forceMode) {
        var vehicle = vehiclesRegistry[vehicleIndex];
        vehicle.Physics.AddRelativeForce(Vector3.back * force, forceMode);
    }

    public void UpdateVehiclePose(int vehicleIndex, Vector3 position, Quaternion rotation) {
        var vehiclePhysics = vehiclesRegistry[vehicleIndex];
        vehiclePhysics.Transform(position, rotation);
    }

    public void MakeTowingConnection(int headVehicleIndex, int tailVehicleIndex) {
        var headVehicle = vehiclesRegistry[headVehicleIndex];
        var tailVehicle = vehiclesRegistry[tailVehicleIndex];
        tailVehicle.Towing.MakeConnection(headVehicle.Chassie);
    }

    public void ClearTowingConnection(int vehicleIndex) {
        var vehicle = vehiclesRegistry[vehicleIndex];
        vehicle.Towing.ClearConnection();
    }

    public VehicleState GetVehicleState(int vehicleId) {
        var vehicle = vehiclesRegistry[vehicleId];
        vehicle.Chassie.GetAxisWheels(WheelAxisName.Front, out var fLeftPos, out var fLeftRot, out var fRightPos, out var fRightRot);
        vehicle.Chassie.GetAxisWheels(WheelAxisName.Rear, out var rLeftPos, out var rLeftRot, out var rRightPos, out var rRightRot);
        return new VehicleState {
            position = vehicle.Position,
            rotation = vehicle.Rotation,
            velocity = vehicle.Velocity,
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