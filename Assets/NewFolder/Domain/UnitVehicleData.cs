using System;

using UnityEngine;

[CreateAssetMenu(fileName = "UnitVehicleData", menuName = "UnitVehicleData", order = 0)]
public class UnitVehicleData : ScriptableObject {
    
    [Serializable]
    public struct VisualsData {
        public GameObject baseGeometry;
        public GameObject wheelGeometry;
    }

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

    public DrivingData drivingData;
    public SoundData soundData;
    public VisualsData visualsData;
    public VehiclePhysicsData physicsData;
}