using UnityEngine;

[CreateAssetMenu(fileName = "CombatConfig", menuName = "CombatConfig", order = 0)]
public class CombatConfig : ScriptableObject {
    public int damage = 1;
    public float attackCooldown = 1f;
    public float attackDuration = 0.3f;
    public float activationRadius = 3f;
}
