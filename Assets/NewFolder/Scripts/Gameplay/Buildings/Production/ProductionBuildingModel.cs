
using UnityEngine;

public class ProductionBuildingModel {
    
    public ProductionBuildingModel(int id, ProductionBuildingConfig config) {
        Id = id;
        Config = config;
    }

    public int Id { get; }
    public ProductionBuildingConfig Config { get; }

    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public int PathfindingObstacleId { get; set; }
    public AvoidanceObstacleId AvoidanceObstacleId { get; set; }
    public CollisionObstacleId CollisionObstacleId { get; set; }
    public HitboxId HitboxId { get; set; }
    public SpawnerId SpawnerId { get; set; }

    public bool Destroyed { get; set; }

}
