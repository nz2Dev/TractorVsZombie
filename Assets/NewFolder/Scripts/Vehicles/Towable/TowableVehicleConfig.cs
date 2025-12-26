using System;

using UnityEngine;

[CreateAssetMenu(fileName = "Towable Vehicle Config", menuName = "Towable Vehicle Config", order = 0)]
public class TowableVehicleConfig : ScriptableObject {
    
    [Serializable]
    public struct RamData {
        public bool enabled;
        public AudioClip[] impactSFX;
        public float radius;
    }

    public RamData ramData;
    public TowableVehicleVisuals visualsPrefab;
    public VehiclePhysics physicsPrefab;
}