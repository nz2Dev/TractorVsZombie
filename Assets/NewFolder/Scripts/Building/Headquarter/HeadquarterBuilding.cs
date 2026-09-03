using Combat;

using UnityEngine;

public class HeadquarterBuilding {

    public HeadquarterBuilding(HeadquarterBuildingConfig config) {
        Config = config;
    }

    public HeadquarterBuildingConfig Config { get; }

    public CombatId CombatId { get; set; }
    public int PathfindingObstacleId { get; set; }
    public AvoidanceObstacleId AvoidanceObstacleId { get; set; }
    public CollisionObstacleId CollisionObstacleId { get; set; }
    public ProximityId ProximityId { get; set; }
    public RaycastId RaycastId { get; set; }
    
    public Vector3 Position { get; set; }
    public bool Destroyed { get; set; }
}