using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WheelRowConstructionInfo {
    public float horizontalOffset;
    public float verticalOffset;
    public float rowOffset;
    public float radius;
    public bool drive;
    public bool stear;
}

[CreateAssetMenu(fileName = "DriveVehicleEntity", menuName = "Entities")]
public class DriveVehicleEntity : ScriptableObject {
    [SerializeField] public Collider baseCollider;
    [SerializeField] public GameObject baseGeometry;
    [SerializeField] public Transform baseGeometryFit;
    [SerializeField] public GameObject wheelGeometry;
    [SerializeField] public Transform wheelGeometryFit;
    [SerializeField] public WheelRowConstructionInfo[] wheelRows;
}
