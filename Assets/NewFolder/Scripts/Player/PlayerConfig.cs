using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlayerConfig", order = 0)]
public class PlayerConfig : ScriptableObject {
    public DriverConfig driverConfig;
    public int maxPlatformCount;
    public PlatformConfig platformConfig;
    [Space]
    public WeaponConfig firstWeaponConfig;
    public WeaponConfig secondWeaponConfig;
}