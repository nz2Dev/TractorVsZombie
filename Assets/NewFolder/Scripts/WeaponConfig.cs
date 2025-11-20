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

    public int bulletDamage;
    public float bulletSpeed;
    public AudioClip[] bulletShootAudioClips;
    public float bulletLifetime;
    public float bulletTravelDistance;
    
    public float rocketFlyDistance;
    public AudioClip[] launchEffectClips;
    public AudioClip[] explodeEffectClips;
    public AnimationCurve rocketFlyCurve;
    public float rocketAmplitude;
    public float rocketFlyDuration;
    public int rocketDamage;
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "WeaponData", order = 0)]
public class WeaponConfig : ScriptableObject {
    public float cooldownSec;
    public Vector3 launchPoint;
    public AimConfig aimConfig;
    public BallisticConfig ballisticConfig;
    public WeaponVisuals visualsPrefab;
}