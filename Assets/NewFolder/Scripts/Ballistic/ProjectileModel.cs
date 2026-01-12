using UnityEngine;

public struct Bullet {
    public Vector3 firePoint;
    public Vector3 velocity;
}

internal class ProjectileModel {
    
    internal ProjectileModel(int id, int shooterId, Vector3 position, Vector3 velocity, float spawnTime, ProjectileConfig config) {
        Id = id;
        ShooterId = shooterId;
        Position = position;
        Velocity = velocity;
        SpawnTime = spawnTime;
        Config = config;
    }

    internal int Id { get; private set; }
    internal int ShooterId { get; private set; }
    internal Vector3 Position { get; private set; }
    internal Vector3 Velocity { get; private set; }
    internal float SpawnTime { get; private set; }
    internal ProjectileConfig Config { get; private set; }
    internal bool IsAged { get; private set; }
    internal bool Killed { get; private set; }

    internal void Move(float deltaTime) {
        Position += Velocity * deltaTime;
    }

    internal void Age(float time) {
        if (!IsAged)
            return;
        
        IsAged = SpawnTime + Config.lifetime < time;
    }

    internal void Kill() {
        Killed = true;
    }
}