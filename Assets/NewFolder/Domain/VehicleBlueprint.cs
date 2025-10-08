using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct VehiclePhysicsData {
    public float mass;
    public Vector3 baseSize;
    public float wheelMass;
    public float forwardFrictionStiffness;
    public float sidewayFrictionStiffness;
    public WheelAxisData[] wheelAxisDatas;
    public float towingTongueLength;
}

[Serializable]
public struct WheelAxisData {
    public float forwardOffset;
    public float upOffset;
    public float halfLength;
    public float radius;
    public bool drive;
    public bool stear;
}

[Serializable]
public struct VehicleVisualsData {
    public GameObject baseGeometry;
    public GameObject wheelGeometry;
    public GameObject towingBodyGeometry;
}

[CreateAssetMenu(fileName = "VehicleBlueprint", menuName = "VehicleBlueprint")]
public class VehicleBlueprint : ScriptableObject {
    public VehicleVisualsData visualsId;
    public VehiclePhysicsData physicsData;
    public float powerAccelerationSpeed = 1;
    public float maxTorque = 50;
    public float maxBreaksTorque = 50;
    public float maxSteerDegrees = 45;
    public AudioClip engineIdleSound;
    public AudioClip[] hitImpactSounds;
    public float ramRadius = 0.5f;
    public float rewardCollectRadius = 2;
}
