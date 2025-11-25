using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlayerConfig", order = 0)]
public class PlayerConfig : ScriptableObject {
    public int maxTrailersCount;
    public VehicleConfig driverConfig;
    public PlatformConfig platformConfig;

    public WeaponConfig firstWeaponConfig;
    public WeaponConfig secondWeaponConfig;

    public float driverRamRadius;
    public float driverRewardCollectRadius;
    public AudioClip[] driverRamImpactSound;
}