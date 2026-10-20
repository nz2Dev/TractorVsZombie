
using UnityEngine;

public class HeadquarterBuilding {

    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public PathfindingObstacleId PathfindingObstacleId { get; set; }
    public AvoidanceObstacleId AvoidanceObstacleId { get; set; }
    public CollisionObstacleId CollisionObstacleId { get; set; }
    public HitboxId HitboxId { get; set; }
    
    public Vector3 Position { get; set; }
    public bool Destroyed { get; set; }
}
