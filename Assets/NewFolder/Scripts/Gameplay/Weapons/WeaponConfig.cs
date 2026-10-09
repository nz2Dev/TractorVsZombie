using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "WeaponData", order = 0)]
public class WeaponConfig : ScriptableObject {
    public float cooldownSec;
    public AimConfig aimConfig;
}