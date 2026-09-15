using System;

using Combat;

using UnityEngine;

public class HeadquarterBuildingController {

    private readonly HeadquarterBuildingView view;
    private readonly CombatSystem combatSystem;
    private readonly PathfindingService pathfindingService;
    private readonly AvoidanceService localAvoidanceService;
    private readonly CollisionService collisionService;
    private readonly RaycastService raycastService;
    private readonly ProximityService proximityService;
    private readonly EntityMapping entityMapping;

    private HeadquarterBuilding headquarter;

    public HeadquarterBuildingController(CombatSystem combatSystem, PathfindingService pathfindingService, CollisionService collisionService, AvoidanceService localAvoidanceService, RaycastService raycastService, EntityMapping entityMapping, ProximityService proximityService, HeadquarterBuildingView view) {
        this.combatSystem = combatSystem;
        this.pathfindingService = pathfindingService;
        this.collisionService = collisionService;
        this.localAvoidanceService = localAvoidanceService;
        this.raycastService = raycastService;
        this.entityMapping = entityMapping;
        this.proximityService = proximityService;
        this.view = view;
    }

    public void Update() {
        ReadCombatOutput();
        CheckLooseCondition();
        UpdateView();
    }

    public void Create(HeadquarterBuildingPrototype prototype) {
        headquarter = new HeadquarterBuilding(prototype.config);
        headquarter.Position = prototype.position;
        headquarter.CombatId = combatSystem.Add(prototype.combatPrototype);
        // headquarter.PathfindingObstacleId = pathfindingService.RegisterObstacle(prototype.pathfindingObstaclePrefab);
        headquarter.AvoidanceObstacleId = localAvoidanceService.AddObstacle(prototype.avoidanceObstaclePrefab);
        headquarter.CollisionObstacleId = collisionService.RegisterObstacle(prototype.position, prototype.collisionObstaclePrefab);
        headquarter.RaycastId = raycastService.RegisterMarker(prototype.position, prototype.raycastMarkerPrefab, CombatSystem.GetRaycastLayerForFaction(prototype.combatPrototype.alie));
        headquarter.ProximityId = proximityService.AddPoint(prototype.position, CombatSystem.GetProximityLayerForFaction(prototype.combatPrototype.alie));

        entityMapping.CreateMappings(new EntityComponents {
            proximityId = headquarter.ProximityId,
            raycastId = headquarter.RaycastId,
            combatId = headquarter.CombatId
        });

        view.ShowHeadquarter(prototype.position, prototype.rotation, prototype.visualsPrefab, prototype.worldSpaceUIPrefab);
    }

    private void ReadCombatOutput() {
        var combatState = combatSystem.ReadState(headquarter.CombatId);
        if (combatState.damageResult?.damageWasFatal == true) {
            headquarter.Destroyed = true;
            
            pathfindingService.UnregisterObstacle(headquarter.PathfindingObstacleId);
            localAvoidanceService.RemoveObstacle(headquarter.AvoidanceObstacleId);
            collisionService.UnregisterObstacle(headquarter.CollisionObstacleId);
            proximityService.RemovePoint(headquarter.ProximityId);
            raycastService.UnregisterMarker(headquarter.RaycastId);

            entityMapping.DeleteMappings(headquarter.ProximityId, headquarter.RaycastId);

            view.ShowHeadquarterDestoryed();
        }
    }

    private void CheckLooseCondition() {
        if (headquarter.Destroyed) {
            Debug.Log("Game over");
        }
    }

    private void UpdateView() {
        var combatState = combatSystem.ReadState(headquarter.CombatId);
        view.UpdateHealth(combatState.health, combatState.maxHealth);
    }

}
