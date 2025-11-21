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

    public SoundData soundData;
    public DrivingData drivingData;
    public WeaponConfig weaponConfig;
    public VehicleVisuals visualsPrefab;
    public VehiclePhysics physicsPrefab;
}