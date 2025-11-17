using System;

using UnityEngine;

[CreateAssetMenu(fileName = "DriverVehicleData", menuName = "DriverVehicleData", order = 0)]
public class DriverVehicleData : ScriptableObject {
    
    [Serializable]
    public struct DrivingData {
        public float maxTorque;
        public float maxBreaksTorque;
        public float maxSteerDegrees;
        public float minStterAmount;
        public float speedCeilingForSteering;
        public float speedKFactor;
        public float powerAccelerationSpeed;
    }

    [Serializable]
    public struct SoundData {
        public AudioClip engineIdleSound;
        public AudioClip[] hitImpactSounds;    
    }

    public float ramRadius;
    public float rewardCollectRadius;
    public DrivingData drivingData;
    public SoundData soundData;
    public VehicleVisuals visualsPrefab;
    public VehiclePhysics physicsPrefab;
}