using UnityEngine;

[CreateAssetMenu(fileName = "RamConfig", menuName = "RamConfig", order = 0)]
public class RamEffectConfig : ScriptableObject {
    public int damage;
    public float triggerRadius;
    public int maxDragInteraction = 5;
    public float maxDragForce = 3000;
    public ForceMode dragForceMode = ForceMode.Impulse;
    public AudioClip[] impactSFX;
    public ExplosionConfig explosionData;
    public float maxImpactSpeed = 2;
}
