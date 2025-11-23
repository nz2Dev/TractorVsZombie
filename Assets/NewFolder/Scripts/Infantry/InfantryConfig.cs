using System;

using UnityEngine;

[Serializable]
public struct InfantryPhysicsConfig {
    public int height;
    public int radius;
}

[CreateAssetMenu(fileName = "InfantryConfig", menuName = "InfantryConfig", order = 0)]
public class InfantryConfig : ScriptableObject {
    public int maxHealth = 5;
    public float attackCooldown = 1;
    public int damage = 1;
    public InfantryVisuals visualsPrefab;
    public WeaponConfig weaponConfig;
    public InfantryPhysicsConfig physicsConfig;
}