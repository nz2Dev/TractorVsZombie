using UnityEngine;

[CreateAssetMenu(fileName = "ArmorConfig", menuName = "ArmorConfig", order = 0)]
public class ArmorConfig : ScriptableObject {
    public int maxHealth = 5;
    
    public VehicleDrivingConfig drivingConfig;
    public ArmorVisuals visualsPrefab;
    public VehiclePhysics physicsPrefab;

    public AudioClip engineLoopSFX;

    public WeaponConfig weaponConfig;
    public Vector3 weaponPlacementOffset;
    public RamConfig ramConfig;
}