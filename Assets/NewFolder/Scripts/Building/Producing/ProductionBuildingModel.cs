using System.Collections.Generic;

using UnityEngine;

public class ProductionBuildingModel {
    
    public int Id { get; }
    public ProductionBuildingConfig Config { get; }
    
    public int CombatId { get; set; }
    public int PathfindingObstacleId { get; set; }
    public int AvoidanceObstacleId { get; set; }
    public int VehicleObstacleId { get; set; }
    public int PhysicsObstacleId { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public bool Alie { get; set; }
    public float NextSpawnTime { get; set; }
    public int QueueAmount { get; set; }
    public bool Destroyed { get; set; }
    public List<int> ProducedEntities { get; } = new();

    public ProductionBuildingModel(int id, ProductionBuildingConfig config) {
        Id = id;
        Config = config;
    }
}
