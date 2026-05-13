using UnityEngine;

[CreateAssetMenu(fileName = "TruckConfig", menuName = "TruckConfig", order = 0)]
public class TruckConfig : ScriptableObject {
    // Driving parameters (maxEngineTorque, maxSteerDegrees, etc.) are configured
    // directly on the VehiclePhysics prefab via its [Header("Driving")] fields.
}