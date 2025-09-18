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

    public int Id { get; set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public Vector3 Velocity { get; private set; }
    public int Health { get; private set; }
    public bool IsAlive { get; private set; }
    public bool Grouned { get; private set; }
    public Damage DeathCause { get; private set; }
    public bool ToBeRemoved { get; private set; }

    private float lastTimeAttacked;

    public Unit(int id, Vector3 position, Quaternion rotation, float maxSpeed, int health = 1) {
        Id = id;
        Position = position;
        Rotation = rotation;
        Velocity = Vector3.zero;
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

    public void Fly(Vector3 position, Quaternion rotation) {
        Position = position;
        Rotation = rotation;
    }

    public void Stand(Vector3 position) {
        Position = position;
        Rotation = Quaternion.identity;
        Grouned = true;
        if (!IsAlive)
            ToBeRemoved = true;
    }

    public void Move(Vector3 position, Quaternion rotation) {
        Position = position;
        Rotation = rotation;
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
        
        Grouned = false;
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