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
        AimForward = Vector3.forward;
        LastShootTime = float.NegativeInfinity;
        this.config = data;
    }

    public int Id { get; private set; }
    public Vector3 Position { get; private set; }
    public Vector3 AimForward { get; private set; }
    public float LastShootTime { get; private set; }

    public int BulletDamage => config.bulletDamage;

    public void Move(Vector3 position) {
        Position = position;
    }

    public void Aim(float deltaTime, Vector3 aimTarget) {
        aimTarget.y = Position.y;
        var positionToAimTarget = aimTarget - Position;
        AimForward = Vector3.Slerp(AimForward, positionToAimTarget.normalized, deltaTime * config.aimSpeed);
    }

    public bool Fire(float time, out Bullet bullet) {
        if (LastShootTime + config.fireCooldown < time) {
            LastShootTime = time;
            bullet = new Bullet {
                firePoint = Position,
                velocity = AimForward * config.bulletSpeed
            };
            return true;
        }
        
        bullet = default;
        return false;
    }
}