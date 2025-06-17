using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WheelAxisData {
    public float forwardOffset;
    public float upOffset;
    public float halfLength;
    public float radius;
    public bool drive;
    public bool stear;
}

[Serializable]
public struct TowingWheelAxisData {
    public float forwardOffset;
    public float upOffset;
    public float halfLength;
    public float radius;
    public float towingBodyLength;
}

[CreateAssetMenu(fileName = "DriveVehicleEntity", menuName = "Entities")]
public class VehicleEntity : ScriptableObject {
    [SerializeField] public Vector3 baseSize = new Vector3(0.5f, 0.4f, 1.0f);
    [SerializeField] public GameObject baseGeometry;
    [SerializeField] public GameObject wheelGeometry;
    [SerializeField] public GameObject towingBodyGeometry;
    [SerializeField] public WheelAxisData[] wheelAxisDatas;
    [SerializeField] public TowingWheelAxisData towingWheelAxisData;
    [SerializeField] public bool towingWheel;

    public TowingWheelAxisData? GetTowingWheelAxisData() {
        if (towingWheel) {
            return towingWheelAxisData;
        } else {
            return null;
        }
    }
}
