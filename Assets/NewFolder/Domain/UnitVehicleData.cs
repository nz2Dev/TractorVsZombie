using System;

using UnityEngine;

[CreateAssetMenu(fileName = "UnitVehicleData", menuName = "UnitVehicleData", order = 0)]
public class UnitVehicleData : ScriptableObject {

    [Serializable]
    public struct DrivingData {
        public float maxTorque;
        public float maxBreaksTorque;
        public float maxSteerDegrees;
        public float powerAccelerationSpeed;
    }

    [Serializable]
    public struct SoundData {
        public AudioClip engineIdleSound;
    }

    [Serializable]
    public struct WeaponsData {
        public RocketLauncherConfig rocketLauncherConfig;
        public TurelConfig turelConfig;
    }

    public WeaponsData weaponsData;
    public DrivingData drivingData;
    public SoundData soundData;
    public VehicleVisuals visualsPrefab;
    public VehiclePhysics physicsPrefab;
}