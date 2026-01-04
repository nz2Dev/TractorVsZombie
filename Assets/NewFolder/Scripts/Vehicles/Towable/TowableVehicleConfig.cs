using System;

using UnityEngine;

[CreateAssetMenu(fileName = "Towable Vehicle Config", menuName = "Towable Vehicle Config", order = 0)]
public class TowableVehicleConfig : ScriptableObject {
    public TowableVehicleVisuals visualsPrefab;
    public VehiclePhysics physicsPrefab;
}