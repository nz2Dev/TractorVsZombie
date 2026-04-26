using UnityEngine;

[CreateAssetMenu(fileName = "RamConfig", menuName = "RamConfig", order = 0)]
public class RamEffectConfig : ScriptableObject {
    public AudioClip[] impactSFX;
    public float radius;
    public float explosionForce;
    public int damage;
}