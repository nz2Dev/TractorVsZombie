using UnityEngine;

[CreateAssetMenu(fileName = "RamConfig", menuName = "RamConfig", order = 0)]
public class RamConfig : ScriptableObject {
    public int damage = 1;
    public float triggerRadius = 3;
    public float dragImpulsePerContact = 0.2f;
    public ExplosionConfig explosionData;
    public float maxImpactSpeed = 2;
    public float minImpactSpeed = 0.5f;
}
