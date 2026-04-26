using UnityEngine;

[CreateAssetMenu(fileName = "TruckConfig", menuName = "TruckConfig", order = 0)]
public class TruckConfig : ScriptableObject {
    public VehicleDrivingConfig drivingConfig;
}