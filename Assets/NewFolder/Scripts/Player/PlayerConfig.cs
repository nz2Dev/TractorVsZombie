using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlayerConfig", order = 0)]
public class PlayerConfig : ScriptableObject {
    public TruckConfig driverConfig;
    public int initPlatformCount;
    public PlatformConfig platformConfig;
    public bool startOrEndCouplingOfRewards = false;
    [Space]
    public WeaponConfig firstWeaponConfig;
    public WeaponConfig secondWeaponConfig;
}