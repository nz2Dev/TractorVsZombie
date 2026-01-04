using UnityEngine;

[CreateAssetMenu(fileName = "ArmorConfig", menuName = "ArmorConfig", order = 0)]
public class ArmorConfig : ScriptableObject {
    public int maxHealth = 5;
    public MotorVehicleConfig vehicleConfig;
    public WeaponConfig weaponConfig;
    public RamConfig ramConfig;
}