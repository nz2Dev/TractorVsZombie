using Combat;

using UnityEngine;

internal class ProjectileModel {

    internal ProjectileModel(int id, ProjectileConfig config, CombatId shooterCombatId, bool shooterAlie) {
        Id = id;
        Config = config;
        ShooterCombatId = shooterCombatId;
        ShooterAlie = shooterAlie;
    }

    internal int Id { get; }
    internal ProjectileConfig Config { get; }
    internal CombatId ShooterCombatId { get; }
    internal bool ShooterAlie { get; }

    internal Vector3 Position { get; set; }
    internal Vector3 Velocity { get; set; }
    internal float SpawnTime { get; set; }
    internal bool IsDead { get; set; }

}