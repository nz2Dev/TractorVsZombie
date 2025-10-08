using System;

using UnityEngine;

public struct WheelAxisPose {
    public Vector3 positionL;
    public Quaternion rotationL;
    public Vector3 positionR;
    public Quaternion rotationR;
}

public struct VehicleBodyPose {
    public Vector3 position;
    public Quaternion rotation;
}

public class Vehicle {

    private readonly VehicleBlueprint blueprint;

    public int Id { get; set; }
    public VehicleBodyPose BodyPose { get; private set; }
    public WheelAxisPose[] WheelAxisPoses { get; private set; }
    public Quaternion? TowingTonqueRotation { get; private set; }

    private float drivePower;
    private float steeringDegrees;

    public Vehicle(VehicleBlueprint blueprint) {
        WheelAxisPoses = new WheelAxisPose[blueprint.physicsData.wheelAxisDatas.Length];
        TowingTonqueRotation = blueprint.physicsData.towingTongueLength > 0 ? default(Quaternion) : null;
        this.blueprint = blueprint;
    }

    public VehiclePhysicsData PhysicsData => blueprint.physicsData;
    public VehicleVisualsData VisualsData => blueprint.visualsId;
    public AudioClip EngineIdleSound => blueprint.engineIdleSound;
    public AudioClip[] HitImpactSounds => blueprint.hitImpactSounds;
    public float DrivePower => drivePower;
    public float MotorTorque => drivePower * blueprint.maxTorque;
    public float SteerDegrees => steeringDegrees;
    public float RamRadius => blueprint.ramRadius;
    public float RewardCollectRadius => blueprint.rewardCollectRadius;

    public void Steer(float steerAmount) {
        this.steeringDegrees = steerAmount * blueprint.maxSteerDegrees;
    }

    public void SteerToward(Vector3 direction) {
        var rotation = BodyPose.rotation;
        var forward = rotation * Vector3.forward;
        var forwardToDirectionDegrees = Vector3.SignedAngle(forward, direction, Vector3.up);
        this.steeringDegrees = Mathf.Clamp(forwardToDirectionDegrees, -blueprint.maxSteerDegrees, blueprint.maxSteerDegrees);
    }

    public void Throttle(float gas, float deltaTime, bool boost) {
        var maxPower = boost ? 2 : 1;
        var accelerationSpeed = boost ? blueprint.powerAccelerationSpeed * 2 : blueprint.powerAccelerationSpeed;
        if (gas > 0) {
            drivePower = Mathf.Lerp(drivePower, maxPower, deltaTime * accelerationSpeed);
        } else {
            drivePower = 0;
        }
    }

    public void OrientBody(VehicleBodyPose bodyPose) {
        BodyPose = bodyPose;
    }

    public void OrientWheelAxis(int index, WheelAxisPose pose) {
        WheelAxisPoses[index] = pose;
    }

    public void OrientTowingTonque(Quaternion rotation) {
        TowingTonqueRotation = rotation;
    }

}