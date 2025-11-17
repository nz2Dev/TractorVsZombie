using System;

using UnityEngine;

[CreateAssetMenu(fileName = "TrailerVehicleData", menuName = "TrailerVehicleData", order = 0)]
public class TrailerVehicleData : ScriptableObject {
    public VehicleVisuals visualsPrefab;
    public VehiclePhysics physicsPrefab;
}