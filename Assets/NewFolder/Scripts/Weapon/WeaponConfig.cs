using System;

using UnityEngine;

[Serializable]
public enum BallisticType {
    Bullet,
    Rocket
}

[Serializable]
public struct BallisticConfig {
    public BallisticType type;
    [Inline] public ProjectileConfig bullet;
    [Inline] public RocketConfig rocket;
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "WeaponData", order = 0)]
public class WeaponConfig : ScriptableObject {
    public float cooldownSec;
    public Vector3 launchOffset;
    public AimConfig aimConfig;
    public BallisticConfig ballisticConfig;
}