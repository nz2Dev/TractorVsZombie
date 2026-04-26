using UnityEngine;

[CreateAssetMenu(fileName = "RamConfig", menuName = "RamConfig", order = 0)]
public class RamConfig : ScriptableObject {
    public AudioClip[] impactSFX;
    public float radius;
    public int damage;
}