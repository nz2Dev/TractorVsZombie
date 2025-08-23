using System;

using UnityEngine;

public class Unit {

    const float PushCooldown = 0.25f;
    
    public int Id { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Velocity { get; set; }
    public float MaxSpeed { get; set; }
    public Vector3 TargetPosition { get; set; }
    public int Health { get; private set; }
    public bool IsAlive => Health >= 0;
    public bool Grouned { get; private set; }

    private float lastTimePushed;

    public Unit(int id, Vector3 position, Quaternion rotation, float maxSpeed, int health = 1) {
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

    public bool TryPush(float time, int damage) {
         if (!Grouned) 
            return false;
                
        if (lastTimePushed + PushCooldown > time) 
            return false;

        lastTimePushed = time;
        TakeDamage(damage);
        return true;
    }

    public void TakeProjectileDamage(int damage) {
        TakeDamage(damage);
    }

    public void SetFlying() {
        Grouned = false;
    }

    internal void SetGrounded() {
        Grouned = true;
    }

    private void TakeDamage(int damageReceived) {
        Health -= damageReceived;
    }
} 