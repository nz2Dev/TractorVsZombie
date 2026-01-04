using System;

using UnityEngine;

[CreateAssetMenu(fileName = "InfantryConfig", menuName = "InfantryConfig", order = 0)]
public class InfantryConfig : ScriptableObject {
    public int maxHealth = 5;
    public float attackCooldown = 1;
    public int damage = 1;
    public BodyConfig bodyConfig;
    public InfantryVisuals visualsPrefab;
}