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
    public BulletConfig bullet;
    public RocketConfig rocket;
}

[Serializable]
public struct BulletConfig {
    public int damage;
    public float speed;
    public AudioClip[] shootAudioClips;
    public float lifetime;
}

[Serializable]
public struct RocketConfig {
    public AudioClip[] launchEffectClips;
    public AudioClip[] explodeEffectClips;
    public AnimationCurve flyCurve;
    public float amplitude;
    public float flyDuration;
    public int damage;
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "WeaponData", order = 0)]
public class WeaponConfig : ScriptableObject {
    public float cooldownSec;
    public Vector3 launchPoint;
    public AimConfig aimConfig;
    public BallisticConfig ballisticConfig;
    public WeaponVisuals visualsPrefab;
}