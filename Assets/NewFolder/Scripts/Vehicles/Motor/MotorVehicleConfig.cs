using System;

using UnityEngine;


[CreateAssetMenu(fileName = "Motor Vehicle Config", menuName = "Motor Vehicle Config", order = 0)]
public class MotorVehicleConfig : ScriptableObject {

    [Serializable]
    public struct DrivingData {
        public float maxTorque;
        public float maxBreaksTorque;
        public float maxSteerDegrees;
        public float minStterAmount;
        public float speedCeilingForSteering;
        public float speedKFactor;
        public float powerAccelerationSpeed;
        public AudioClip engineIdleSound;
    }

    [Serializable]
    public struct RamData {
        public bool enabled;
        public AudioClip[] impactSFX;
        public float radius;
    }

    public RamData ramData;
    public DrivingData drivingData;
    public MotorVehicleVisuals visualsPrefab;
    public VehiclePhysics physicsPrefab;

}