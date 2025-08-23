using System;

using UnityEngine;

public struct RayDamage {
    public int amount;
    public Vector3 sourcePosition;
    public Vector3 rayDirection;
}

public class Turel {

    public Turel(int id) {
        Id = id;
    }

    public int Id { get; private set; }
    public Vector3 Position { get; private set; }
    public Vector3 AimForward { get; private set; }
    public float AimSpeed { get; private set; }
    public int Ammo { get; private set; }
    public int Damage { get; private set; }
    public float LastShootTime { get; private set; }
    public float ShootColdown { get; private set; }

    public void Move(Vector3 position) {
        Position = position;
    }

    public void Aim(Vector3 aimTarget) {
        var positionToAimTarget = aimTarget - Position;
        AimForward = Vector3.Slerp(AimForward, positionToAimTarget.normalized, Time.time * AimSpeed);
    }

    public bool Shoot(float time, out RayDamage damage) {
        if (LastShootTime + ShootColdown < time) {
            LastShootTime = time;
            damage = new RayDamage {
                amount = Damage,
                sourcePosition = Position,
                rayDirection = AimForward
            };
            return true;
        }
        
        damage = default;
        return false;
    }
}