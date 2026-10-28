using System.Collections.Generic;

public sealed class StructureSystem {

    private readonly AvoidanceService avoidanceService;
    private readonly CollisionService collisionService;

    private int idCounter;
    private readonly Dictionary<StructureId, StructureModel> models = new();

    public StructureSystem(AvoidanceService avoidanceService, CollisionService collisionService) {
        this.avoidanceService = avoidanceService;
        this.collisionService = collisionService;
    }

    public StructureId Add(StructurePrototype prototype) {
        var id = new StructureId(++idCounter);
        var avoidanceObstacleId = avoidanceService.AddObstacle(prototype.avoidanceObstaclePrefab);
        var collisionObstacleId = collisionService.RegisterObstacle(prototype.position, prototype.collisionObstaclePrefab);
        // var pathfindingObstacleId = pathfindingService.RegisterObstacle(prototype.pathfindingObstaclePrefab);
        models.Add(id, new StructureModel(id, avoidanceObstacleId, collisionObstacleId));
        return id;
    }

    public void Remove(StructureId id) {
        models.Remove(id, out var model);
        avoidanceService.RemoveObstacle(model.AvoidanceObstacleId);
        collisionService.UnregisterObstacle(model.CollisionObstacleId);
        // pathfindingService.UnregisterObstacle(headquarter.PathfindingObstacleId);
    }
}
