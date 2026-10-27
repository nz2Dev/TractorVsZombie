internal sealed class StructureModel {
    
    public StructureId Id { get; }
    public AvoidanceObstacleId AvoidanceObstacleId { get; }
    public CollisionObstacleId CollisionObstacleId { get; }
    // public PathfindingObstacleId PathfindingObstacleId { get; set; }

    public StructureModel(StructureId id, AvoidanceObstacleId avoidanceObstacleId, CollisionObstacleId collisionObstacleId) {
        Id = id;
        AvoidanceObstacleId = avoidanceObstacleId;
        CollisionObstacleId = collisionObstacleId;
    }
}
