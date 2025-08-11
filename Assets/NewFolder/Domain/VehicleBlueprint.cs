using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct VehiclePhysicsData {
    public float mass;
    public Vector3 baseSize;
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
}
