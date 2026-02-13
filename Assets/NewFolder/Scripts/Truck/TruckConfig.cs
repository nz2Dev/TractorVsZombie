using UnityEngine;

[CreateAssetMenu(fileName = "TruckConfig", menuName = "TruckConfig", order = 0)]
public class TruckConfig : ScriptableObject {
    public RamConfig ramConfig;
    public VehicleDrivingConfig drivingConfig;
    public VehiclePhysics vehiclePhysicsPrefab;
    public TruckVisuals visualsPrefab;
    public AudioClip engineLoopSFX;
}