using System;

using UnityEngine;

[CreateAssetMenu(fileName = "InfantryConfig", menuName = "InfantryConfig", order = 0)]
public class InfantryConfig : ScriptableObject {
    public bool alie;          // but this
    public int maxHealth = 5; // and this, is still a component (combat)

    public float attackCooldown = 1; // this one might be the infantry domain data
    public int damage = 1;
    
    public BodyData bodyData; // those are the component (physics)
    public AgentAvoidanceConfig agentAvoidanceConfig; // and this are the component (avoidance)
}