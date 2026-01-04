using UnityEngine;

[CreateAssetMenu(fileName = "InnertialWeaponConfig", menuName = "InnertialWeaponConfig", order = 0)]
public class RamConfig : ScriptableObject {
    public AudioClip[] impactSFX;
    public float radius;
}