using UnityEngine;

[CreateAssetMenu(fileName = "TurelData", menuName = "TurelData", order = 0)]
public class TurelConfig : ScriptableObject {
    public float aimSpeed = 1;
    public float fireCooldown = 0.25f;
    public int bulletDamage = 1;
    public float bulletSpeed = 15f;
    public float gunHeight = 0.5f;
    public float bulletLifetime = 3;
}