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

[CreateAssetMenu(fileName = "DriveVehicleEntity", menuName = "Entities")]
public class VehicleEntity : ScriptableObject {
    [SerializeField] public Collider baseCollider;
    [SerializeField] public GameObject baseGeometry;
    [SerializeField] public Transform baseGeometryFit;
    [SerializeField] public GameObject wheelGeometry;
    [SerializeField] public Transform wheelGeometryFit;
    [SerializeField] public WheelAxisData[] wheelAxisDatas;
}
