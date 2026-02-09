using UnityEngine;

[CreateAssetMenu(fileName = "DriverConfig", menuName = "DriverConfig", order = 0)]
public class TruckConfig : ScriptableObject {
    public MotorVehicleConfig vehicleConfig;
    public RamConfig ramConfig;
}