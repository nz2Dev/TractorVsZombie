using System;

using UnityEngine;

public class Unit {

    public struct Damage {
        public DamageType type;
        public Vector3 damageSource;
        public int amount;
    }

    public enum DamageType {
        Physical,
        Explosion,
        Projectile
    }

    const float PushCooldown = 0.25f;
    
    public int Id { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Velocity { get; set; }
    public float MaxSpeed { get; set; }
    public Vector3 TargetPosition { get; set; }
    public int Health { get; private set; }
    public bool IsAlive { get; private set; }
    public bool Grouned { get; private set; }
    public Damage DeathCause { get; private set; }
    public bool ToBeRemoved { get; private set; }

    private float lastTimeExploded;
    private float lastTimeAttacked;

    public Unit(int id, Vector3 position, Quaternion rotation, float maxSpeed, int health = 1) {
        Id = id;
        Position = position;
        Rotation = rotation;
        MaxSpeed = maxSpeed;
        Velocity = Vector3.zero;
        TargetPosition = position;
        Health = health;
        IsAlive = true;
    }

    public bool TryDirectFrontAttack(float atTime, out int damage) {
        bool lastTimeAttackIsInThePast = lastTimeAttacked + 1 < atTime;
        if (lastTimeAttackIsInThePast) {
            lastTimeAttacked = atTime;
            damage = 10;
            return true;
        }
        damage = 0;
        return false;
    }

    public void ForceKill() {
        Health = 0;
    }

    public void SetFlying() {
        Grouned = false;
    }

    internal void SetGrounded() {
        Grouned = true;
        if (!IsAlive) {
            ToBeRemoved = true;
        }
    }

    public void TakeProjectileHit(int damage, Vector3 projectilePosition) {
        TakeDamage(damage, new Damage {
            amount = damage,
            damageSource = projectilePosition,
            type = DamageType.Projectile
        });
    }

    public bool TryTakeExplosionHit(float time, int damage, Vector3 explosionEpicentr) {
         if (!Grouned) 
            return false;
                
        if (lastTimeExploded + PushCooldown > time) 
            return false;

        lastTimeExploded = time;
        TakeDamage(damage, new Damage {
            amount = damage,
            damageSource = explosionEpicentr,
            type = DamageType.Explosion
        });
        return true;
    }

    private void TakeDamage(int damageReceived, Damage damage) {
        Health -= damageReceived;
        
        if (Health <= 0) {
            Health = 0;
            IsAlive = false;
            DeathCause = damage;
            ToBeRemoved = true && Grouned; 
        }
    }

} 