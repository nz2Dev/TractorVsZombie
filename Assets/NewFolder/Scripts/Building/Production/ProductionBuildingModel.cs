using Combat;

using UnityEngine;

public class ProductionBuildingModel {
    
    public ProductionBuildingModel(int id, ProductionBuildingConfig config, SpawnSpot spawnSpot, SpawnVariant spawnVariant) {
        Id = id;
        Config = config;
        SpawnSpot = spawnSpot;
        SpawnVariant = spawnVariant;
    }

    public int Id { get; }
    public ProductionBuildingConfig Config { get; }
    public SpawnSpot SpawnSpot { get; }
    public SpawnVariant SpawnVariant { get; }

    public CombatId CombatId { get; set; }
    public int PathfindingObstacleId { get; set; }
    public int AvoidanceObstacleId { get; set; }
    public int VehicleObstacleId { get; set; }
    public int PhysicsObstacleId { get; set; }

    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public int QueueAmount { get; set; }
    public float NextSpawnTime { get; set; }
    public bool Destroyed { get; set; }
    public SpawnResult SpawnResult { get; set; }

}
