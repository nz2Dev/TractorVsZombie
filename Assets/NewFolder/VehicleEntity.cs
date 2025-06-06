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
    [SerializeField] public Vector3 baseSize = new Vector3(0.5f, 0.4f, 1.0f);
    [SerializeField] public GameObject baseGeometry;
    [SerializeField] public GameObject wheelGeometry;
    [SerializeField] public WheelAxisData[] wheelAxisDatas;
}
