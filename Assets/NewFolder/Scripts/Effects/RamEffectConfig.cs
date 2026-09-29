using UnityEngine;

[CreateAssetMenu(fileName = "RamConfig", menuName = "RamConfig", order = 0)]
public class RamEffectConfig : ScriptableObject {
    public int damage;
    public float triggerRadius;
    public float temporalLinearDragPerContact;
    public float maxTemporalLinearDrag;
    public float temporalLinearDragRecoverySpeed;
    public AudioClip[] impactSFX;
    public ExplosionConfig explosionData;
    public float maxImpactSpeed = 2;
    public float minImpactSpeed = 0.5f;
}
