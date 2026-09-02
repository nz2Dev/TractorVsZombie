using Combat;

using UnityEngine;

public class HeadquarterBuildingController {

    private readonly CombatSystem combatSystem;
    private readonly PathfindingService pathfindingService;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly CollisionService collisionService;
    private readonly RaycastService raycastService;
    private readonly ProximityService proximityService;
    private readonly EntityMapping entityMapping;

    private GameObject visuals;
    private HeadquarterBuilding headquarter;

    public HeadquarterBuildingController(CombatSystem combatSystem, PathfindingService pathfindingService, CollisionService collisionService, LocalAvoidanceService localAvoidanceService, RaycastService raycastService, EntityMapping entityMapping, ProximityService proximityService) {
        this.combatSystem = combatSystem;
        this.pathfindingService = pathfindingService;
        this.collisionService = collisionService;
        this.localAvoidanceService = localAvoidanceService;
        this.raycastService = raycastService;
        this.entityMapping = entityMapping;
        this.proximityService = proximityService;
    }

    public void Update() {
        ReadCombatOutput();
        CheckLooseCondition();
    }

    public void Create(HeadquarterBuildingPrototype prototype) {
        headquarter = new HeadquarterBuilding(prototype.config);
        headquarter.Position = prototype.position;
        headquarter.CombatId = combatSystem.Add(prototype.combatPrototype);
        headquarter.PathfindingObstacleId = pathfindingService.RegisterObstacle(prototype.position, (int) 4/* TODO: prototype.pathfindingObstacle*/); // need separate component for this
        headquarter.AvoidanceObstacleId = localAvoidanceService.AddObstacle(prototype.position, prototype.rotation, prototype.avoidanceObstaclePrefab);
        headquarter.CollisionObstacleId = collisionService.RegisterObstacle(prototype.position, prototype.collisionObstaclePrefab);
        headquarter.RaycastId = raycastService.RegisterMarker(prototype.position, prototype.raycastMarkerPrefab, CombatSystem.GetRaycastLayerForFaction(prototype.combatPrototype.alie));
        headquarter.ProximityId = proximityService.AddPoint(prototype.position, CombatSystem.GetProximityLayerForFaction(prototype.combatPrototype.alie));

        entityMapping.CreateMappings(new EntityComponents {
            proximityId = headquarter.ProximityId,
            raycastId = headquarter.RaycastId,
            combatId = headquarter.CombatId
        });

        visuals = GameObject.Instantiate(prototype.visualsPrefab, prototype.position, prototype.rotation);
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

            GameObject.Destroy(visuals);
        }
    }

    private void CheckLooseCondition() {
        if (headquarter.Destroyed) {
            Debug.Log("Game over");
        }
    }

}
