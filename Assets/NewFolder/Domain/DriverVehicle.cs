using System;

using UnityEngine;

public class DriverVehicle {

    private readonly DriverVehicleData data;

    public VehicleBodyPose BodyPose { get; private set; }
    public WheelAxisPose[] WheelAxisPoses { get; private set; }

    private float drivePower;
    private float steeringDegrees;
    private float breaksPower;

    public DriverVehicle(DriverVehicleData data) {
        WheelAxisPoses = new WheelAxisPose[data.physicsData.wheelAxisDatas.Length];
        this.data = data;
    }

    public VehiclePhysicsData PhysicsData => data.physicsData;
    public DriverVehicleData.VisualsData VisualsData => data.visualsData;
    public AudioClip EngineIdleSound => data.soundData.engineIdleSound;
    public AudioClip[] HitImpactSounds => data.soundData.hitImpactSounds;
    
    public float DrivePower => drivePower;
    public float BreaksTorque => breaksPower * data.drivingData.maxBreaksTorque;
    public float MotorTorque => drivePower * data.drivingData.maxTorque;
    public float SteerDegrees => steeringDegrees;
    public float RamRadius => data.ramRadius;
    public float RewardCollectRadius => data.rewardCollectRadius;

    public void Steer(float steerAmount) {
        this.steeringDegrees = steerAmount * data.drivingData.maxSteerDegrees;
    }

    public void SteerToward(Vector3 direction) {
        var rotation = BodyPose.rotation;
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

    public void OrientBody(VehicleBodyPose bodyPose) {
        BodyPose = bodyPose;
    }

    public void OrientWheelAxis(int index, WheelAxisPose pose) {
        WheelAxisPoses[index] = pose;
    }

}