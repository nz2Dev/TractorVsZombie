
using UnityEngine;

public class HeadquarterBuilding {

    public HeadquarterBuilding(HeadquarterBuildingConfig config) {
        Config = config;
    }

    public HeadquarterBuildingConfig Config { get; }

    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public PathfindingObstacleId PathfindingObstacleId { get; set; }
    public AvoidanceObstacleId AvoidanceObstacleId { get; set; }
    public CollisionObstacleId CollisionObstacleId { get; set; }
    public ProximityId ProximityId { get; set; }
    public RaycastId RaycastId { get; set; }
    
    public Vector3 Position { get; set; }
    public bool Destroyed { get; set; }
}