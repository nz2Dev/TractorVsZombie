using System;

using UnityEngine;

public struct RayDamage {
    public int amount;
    public Vector3 sourcePosition;
    public Vector3 velocity;
}

public class Turel {

    public Turel(int id) {
        Id = id;
    }

    public int Id { get; private set; }
    public Vector3 Position { get; private set; }
    public Vector3 AimForward { get; private set; } = Vector3.forward;
    public float AimSpeed { get; private set; } = 1;
    public int Ammo { get; private set; }
    public int Damage { get; private set; } = 5;
    public float LastShootTime { get; private set; }
    public float ShootColdown { get; private set; } = .25f;

    public void Move(Vector3 position) {
        Position = position;
    }

    public void Aim(float deltaTime, Vector3 aimTarget) {
        aimTarget.y = Position.y;
        var positionToAimTarget = aimTarget - Position;
        AimForward = Vector3.Slerp(AimForward, positionToAimTarget.normalized, deltaTime * AimSpeed);
    }

    public bool IsAligned(Vector3 point) {
        return Vector3.Dot(AimForward, (point - Position).normalized) > 0.98f;
    }

    public bool Shoot(float time, out RayDamage damage) {
        if (LastShootTime + ShootColdown < time) {
            LastShootTime = time;
            damage = new RayDamage {
                amount = Damage,
                sourcePosition = Position,
                velocity = AimForward * 15
            };
            return true;
        }
        
        damage = default;
        return false;
    }
}