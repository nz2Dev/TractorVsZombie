using System.Collections.Generic;

public sealed class StructureSystem {
    private readonly AvoidanceService avoidanceService;
    private readonly CollisionService collisionService;
    private readonly Dictionary<StructureId, StructureModel> models = new();
    private int idCounter;

    public StructureSystem(AvoidanceService avoidanceService, CollisionService collisionService) {
        this.avoidanceService = avoidanceService;
        this.collisionService = collisionService;
    }

    public StructureId Add(StructurePrototype prototype) {
        var id = new StructureId(++idCounter);
        var avoidanceObstacleId = avoidanceService.AddObstacle(prototype.avoidanceObstaclePrefab);
        var collisionObstacleId = collisionService.RegisterObstacle(prototype.position, prototype.collisionObstaclePrefab);
        models.Add(id, new StructureModel(id, avoidanceObstacleId, collisionObstacleId));
        return id;
    }

    public void Remove(StructureId id) {
        if (!models.Remove(id, out var model))
            return;

        avoidanceService.RemoveObstacle(model.AvoidanceObstacleId);
        collisionService.UnregisterObstacle(model.CollisionObstacleId);
    }
}
