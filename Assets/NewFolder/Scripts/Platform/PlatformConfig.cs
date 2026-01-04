using UnityEngine;

[CreateAssetMenu(fileName = "PlatformConfig", menuName = "PlatformConfig", order = 0)]
public class PlatformConfig : ScriptableObject {
    public TowableVehicleConfig vehicleConfig;
    public RamConfig ramConfig;
}