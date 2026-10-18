using System;


using UnityEngine;

public class HeadquarterBuildingController {

    private readonly HeadquarterBuildingView view;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly PathfindingService pathfindingService;
    private readonly AvoidanceService localAvoidanceService;
    private readonly CollisionService collisionService;
    private readonly HitboxSystem hitboxSystem;
    private readonly EntityMapping entityMapping;

    private HeadquarterBuilding headquarter;

    public HeadquarterBuildingController(HealthSystem healthSystem, FactionRegistry factionRegistry, PathfindingService pathfindingService, CollisionService collisionService, AvoidanceService localAvoidanceService, HitboxSystem hitboxSystem, EntityMapping entityMapping, HeadquarterBuildingView view) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.pathfindingService = pathfindingService;
        this.collisionService = collisionService;
        this.localAvoidanceService = localAvoidanceService;
        this.hitboxSystem = hitboxSystem;
        this.entityMapping = entityMapping;
        this.view = view;
    }

    public void Update() {
        ReadHealthOutput();
        UpdateView();
    }

    public void Create(HeadquarterBuildingPrototype prototype) {
        headquarter = new HeadquarterBuilding(prototype.config);
        headquarter.Position = prototype.position;
        headquarter.HealthId = healthSystem.Add(prototype.healthPrototype);
        headquarter.FactionId = factionRegistry.Add(prototype.factionPrototype);
        // headquarter.PathfindingObstacleId = pathfindingService.RegisterObstacle(prototype.pathfindingObstaclePrefab);
        headquarter.AvoidanceObstacleId = localAvoidanceService.AddObstacle(prototype.avoidanceObstaclePrefab);
        headquarter.CollisionObstacleId = collisionService.RegisterObstacle(prototype.position, prototype.collisionObstaclePrefab);
        headquarter.HitboxId = hitboxSystem.Add(headquarter.FactionId, prototype.position, prototype.raycastMarkerPrefab);

        entityMapping.CreateMapping(headquarter.HitboxId, new EntityComponents {
            healthId = headquarter.HealthId,
            factionId = headquarter.FactionId
        });

        view.ShowHeadquarter(prototype.position, prototype.rotation, prototype.visualsPrefab, prototype.worldSpaceUIPrefab);
    }

    public bool IsDestroyed() {
        return headquarter.Destroyed;
    }

    private void ReadHealthOutput() {
        var healthState = healthSystem.ReadState(headquarter.HealthId);
        if (healthState.damageResult.HasValue) {
            view.ShowTakeHit();
        }

        if (healthState.damageResult?.damageWasFatal == true) {
            headquarter.Destroyed = true;
            
            pathfindingService.UnregisterObstacle(headquarter.PathfindingObstacleId);
            localAvoidanceService.RemoveObstacle(headquarter.AvoidanceObstacleId);
            collisionService.UnregisterObstacle(headquarter.CollisionObstacleId);
            hitboxSystem.Remove(headquarter.HitboxId);

            entityMapping.DeleteMapping(headquarter.HitboxId);
            healthSystem.Remove(headquarter.HealthId);
            factionRegistry.Remove(headquarter.FactionId);

            view.ShowHeadquarterDestoryed();
        }
    }

    private void UpdateView() {
        var healthState = healthSystem.ReadState(headquarter.HealthId);
        view.UpdateHealth(healthState.health, healthState.maxHealth);
    }

}
