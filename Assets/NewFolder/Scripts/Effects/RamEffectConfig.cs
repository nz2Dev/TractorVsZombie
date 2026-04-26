using UnityEngine;

[CreateAssetMenu(fileName = "RamConfig", menuName = "RamConfig", order = 0)]
public class RamEffectConfig : ScriptableObject {
    public AudioClip[] impactSFX;
    public int damage;
    public float triggerRadius;
    public ExplosionData explosionData;
}