using System;

using UnityEngine;

[Serializable]
public struct SoundData {
    public AudioClip engineIdleSound;
}

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
public struct RamData {
    public bool enabled;
    public AudioClip[] impactSFX;
    public float radius;
}

[CreateAssetMenu(fileName = "VehicleConfig", menuName = "VehicleConfig", order = 0)]
public class VehicleConfig : ScriptableObject {
    public RamData ramData;
    public SoundData soundData;
    public DrivingData drivingData;
    public VehicleVisuals visualsPrefab;
    public VehiclePhysics physicsPrefab;

}