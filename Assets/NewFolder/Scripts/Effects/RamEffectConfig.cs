using UnityEngine;

[CreateAssetMenu(fileName = "RamConfig", menuName = "RamConfig", order = 0)]
public class RamEffectConfig : ScriptableObject {
    public int damage;
    public float triggerRadius;
    public AudioClip[] impactSFX;
    public ExplosionConfig explosionData;
}
