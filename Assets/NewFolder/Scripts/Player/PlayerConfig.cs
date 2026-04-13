using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlayerConfig", order = 0)]
public class PlayerConfig : ScriptableObject {
    public TruckConfig driverConfig;
    public int initPlatformCount;
    public PlatformConfig platformConfig;
    public bool startOrEndCouplingOfRewards = false;
    [Space]
    public GameObject brokenArmorVisualsPrefab;
    public WeaponConfig firstWeaponConfig;
    public Vector3 firstWeaponOffset;
    public WeaponConfig secondWeaponConfig;
    public Vector3 secondWeaponOffset;
}