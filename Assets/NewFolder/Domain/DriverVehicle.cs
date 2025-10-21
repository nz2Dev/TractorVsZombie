using System;

using UnityEngine;

public class DriverVehicle {

    private readonly DriverVehicleData data;

    public VehicleBodyPose BodyPose { get; private set; }
    public WheelAxisPose[] WheelAxisPoses { get; private set; }

    private float drivePower;
    private float steering;
    private float steerLimit;
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
    public float SteerDegrees => steering * (data.drivingData.maxSteerDegrees * steerLimit);
    public float RamRadius => data.ramRadius;
    public float RewardCollectRadius => data.rewardCollectRadius;

    public void Steer(float steerAmount) {
        float t = Mathf.Clamp01(BodyPose.velocity.magnitude / data.drivingData.speedCeilingForSteering);
        float steerFactor = 1f - Mathf.Pow(t, data.drivingData.speedKFactor); // k > 1 makes the falloff sharper near top speed
        steerLimit = Mathf.Max(data.drivingData.minStterAmount, steerFactor);
        this.steering = steerAmount;
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