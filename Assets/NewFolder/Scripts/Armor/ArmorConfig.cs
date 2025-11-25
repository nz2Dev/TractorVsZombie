using UnityEngine;

[CreateAssetMenu(fileName = "ArmorConfig", menuName = "ArmorConfig", order = 0)]
public class ArmorConfig : ScriptableObject {
    public int maxHealth = 5;
    public VehicleConfig vehicleConfig;
    public WeaponConfig weaponConfig;
    public bool applyRamDamage;
    public AudioClip[] ramImpactSFX;
    public float ramRadius;
}