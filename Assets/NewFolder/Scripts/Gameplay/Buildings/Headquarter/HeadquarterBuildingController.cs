using System;


using UnityEngine;

public class HeadquarterBuildingController {

    private readonly HeadquarterBuildingView view;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly PathfindingService pathfindingService;
    private readonly AvoidanceService localAvoidanceService;
    private readonly CollisionService collisionService;
    private readonly RaycastService raycastService;
    private readonly ProximityService proximityService;
    private readonly EntityMapping entityMapping;

    private HeadquarterBuilding headquarter;

    public HeadquarterBuildingController(HealthSystem healthSystem, FactionRegistry factionRegistry, PathfindingService pathfindingService, CollisionService collisionService, AvoidanceService localAvoidanceService, RaycastService raycastService, EntityMapping entityMapping, ProximityService proximityService, HeadquarterBuildingView view) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.pathfindingService = pathfindingService;
        this.collisionService = collisionService;
        this.localAvoidanceService = localAvoidanceService;
        this.raycastService = raycastService;
        this.entityMapping = entityMapping;
        this.proximityService = proximityService;
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
        var alie = factionRegistry.ReadState(headquarter.FactionId).alie;
        headquarter.RaycastId = raycastService.RegisterMarker(prototype.position, prototype.raycastMarkerPrefab, FactionRegistry.GetRaycastLayerForFaction(alie));
        headquarter.ProximityId = proximityService.AddPoint(prototype.position, FactionRegistry.GetProximityLayerForFaction(alie));

        entityMapping.CreateMappings(new EntityComponents {
            proximityId = headquarter.ProximityId,
            raycastId = headquarter.RaycastId,
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
            proximityService.RemovePoint(headquarter.ProximityId);
            raycastService.UnregisterMarker(headquarter.RaycastId);

            entityMapping.DeleteMappings(headquarter.ProximityId, headquarter.RaycastId);
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
