using UnityEngine;

internal class ProjectileModel {
    
    internal int Id { get; }
    internal ProjectileConfig Config { get; }
    internal int ShooterCombatId { get; }

    internal ProjectileModel(int id, ProjectileConfig config, int shooterCombatId) {
        Id = id;
        Config = config;
        ShooterCombatId = shooterCombatId;
    }

    internal Vector3 Position { get; set; }
    internal Vector3 Velocity { get; set; }
    internal float SpawnTime { get; set; }
    internal bool IsAlive { get; set; }

}