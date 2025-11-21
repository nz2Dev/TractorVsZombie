using System;

using UnityEngine;

public class UnitVehicle {

    private readonly UnitVehicleData data;

    private VehicleState vehicleState;

    private float drivePower;
    private float steeringDegrees;
    private float breaksPower;

    public UnitVehicle(UnitVehicleData data) {
        this.data = data;
        Health = 5;
    }

    public int Health { get; private set; }    
    public bool IsAlive => Health > 0;
    public VehiclePhysics PhysicsPrefab => data.physicsPrefab;
    public VehicleVisuals VisualsPrefab => data.visualsPrefab;
    public AudioClip EngineIdleSound => data.soundData.engineIdleSound;
    public WeaponConfig WeaponsConfig => data.weaponConfig;

    public Vector3 Position => vehicleState.position;    
    public VehicleState PhysicsState => vehicleState;
    public float DrivePower => drivePower;
    public float BreaksTorque => breaksPower * data.drivingData.maxBreaksTorque;
    public float MotorTorque => drivePower * data.drivingData.maxTorque;
    public float SteerDegrees => steeringDegrees;

    public void UpdatePhysicsState(VehicleState state) {
        vehicleState = state;
    }

    public void SteerToward(Vector3 direction) {
        var rotation = vehicleState.rotation;
        var forward = rotation * Vector3.forward;
        var forwardToDirectionDegrees = Vector3.SignedAngle(forward, direction, Vector3.up);
        this.steeringDegrees = Mathf.Clamp(forwardToDirectionDegrees, -data.drivingData.maxSteerDegrees, data.drivingData.maxSteerDegrees);
    }

    public void Throttle(float gas, float deltaTime, bool boost) {
        var maxPower = boost ? 2 : 1;
        var accelerationSpeed = boost ? data.drivingData.powerAccelerationSpeed * 2 : data.drivingData.powerAccelerationSpeed;
        if (gas > 0) {
            drivePower = Mathf.Lerp(drivePower, maxPower, deltaTime * accelerationSpeed);
        } else {
            drivePower = 0;
        }
    }

    public void Breaks(float breakingAmount) {
        this.breaksPower = breakingAmount;
    }

    internal void TakeDamage(int damage) {
        Health -= damage;
    }
}