
using UnityEngine;

internal class ProjectileModel {

    internal ProjectileModel(int id, ProjectileConfig config, FactionId shooterFactionId, bool shooterAlie) {
        Id = id;
        Config = config;
        ShooterFactionId = shooterFactionId;
        ShooterAlie = shooterAlie;
    }

    internal int Id { get; }
    internal ProjectileConfig Config { get; }
    internal FactionId ShooterFactionId { get; }
    internal bool ShooterAlie { get; }

    internal Vector3 Position { get; set; }
    internal Vector3 Velocity { get; set; }
    internal float SpawnTime { get; set; }
    internal bool IsDead { get; set; }

}
