using System;

using UnityEngine;

[CreateAssetMenu(fileName = "InfantryConfig", menuName = "InfantryConfig", order = 0)]
public class InfantryConfig : ScriptableObject {
    public int damage = 1;
    public float attackCooldown = 1; // this one might be the infantry domain data
}