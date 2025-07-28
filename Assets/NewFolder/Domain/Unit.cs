using System;

using UnityEngine;

public class Unit {
    
    public int Id { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Velocity { get; set; }
    public float MaxSpeed { get; set; }
    public Vector3 TargetPosition { get; set; }
    public int Health { get; private set; }
    public bool IsAlive => Health >= 0;
    public bool Grouned { get; private set; }

    public Unit(int id, Vector3 position, Quaternion rotation, float maxSpeed, int health = 100) {
        Id = id;
        Position = position;
        Rotation = rotation;
        MaxSpeed = maxSpeed;
        Velocity = Vector3.zero;
        TargetPosition = position;
        Health = health;
    }

    public void ForceKill() {
        Health = 0;
    }

    public void TakeDamage(int damageReceived) {
        Health -= damageReceived;
    }

    public void SetFlying() {
        Grouned = false;
    }

    internal void SetGrounded() {
        Grouned = true;
    }
} 