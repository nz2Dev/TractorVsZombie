using System;

using UnityEngine;

[CreateAssetMenu(fileName = "TrailerVehicleData", menuName = "TrailerVehicleData", order = 0)]
public class TrailerVehicleData : ScriptableObject {
    
    [Serializable]
    public struct VisualsData {
        public GameObject baseGeometry;
        public GameObject wheelGeometry;
        public GameObject towingBodyGeometry;
    }

    public VisualsData visualsData;
    public VehiclePhysicsData physicsData;
}