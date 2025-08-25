using System;

using UnityEngine;

public struct Bullet {
    public Vector3 firePoint;
    public Vector3 velocity;
}

public class Turel {

    private readonly TurelConfig config;

    public Turel(int id, Vector3 position, TurelConfig data) {
        Id = id;
        Position = position;
        GunForward = Vector3.forward;
        LastShootTime = float.NegativeInfinity;
        this.config = data;
    }

    public int Id { get; private set; }
    public Vector3 Position { get; private set; }
    public Vector3 GunForward { get; private set; }
    public float LastShootTime { get; private set; }

    public int BulletDamage => config.bulletDamage;

    public void Move(Vector3 position) {
        Position = position;
    }

    public void Aim(float deltaTime, Vector3 aimTarget) {
        var gunPoint = Position + Vector3.up * config.gunHeight;
        var gunToTarget = aimTarget - gunPoint;
        GunForward = Vector3.Slerp(GunForward, gunToTarget.normalized, deltaTime * config.aimSpeed);
    }

    public bool Fire(float time, out Bullet bullet) {
        if (LastShootTime + config.fireCooldown < time) {
            LastShootTime = time;
            bullet = new Bullet {
                firePoint = Position + Vector3.up * config.gunHeight,
                velocity = GunForward * config.bulletSpeed
            };
            return true;
        }
        
        bullet = default;
        return false;
    }
}