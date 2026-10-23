using UnityEngine;

[CreateAssetMenu(fileName = "RamConfig", menuName = "RamConfig", order = 0)]
public class RamConfig : ScriptableObject {
    public int damage;
    public float triggerRadius;
    public float temporalLinearDragPerContact;
    public ExplosionConfig explosionData;
    public float maxImpactSpeed = 2;
    public float minImpactSpeed = 0.5f;
}
