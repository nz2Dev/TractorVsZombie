using Combat;

using UnityEngine;

public class HeadquarterBuildingController {

    private readonly CombatSystem combatSystem;
    private readonly PathfindingService pathfindingService;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly VehicleService vehicleService;
    private readonly RagdollService physicsService;

    private GameObject visuals;
    private HeadquarterBuilding headquarter;

    public HeadquarterBuildingController(CombatSystem combatSystem, PathfindingService pathfindingService, VehicleService vehicleService, RagdollService physicsService, LocalAvoidanceService localAvoidanceService) {
        this.combatSystem = combatSystem;
        this.pathfindingService = pathfindingService;
        this.vehicleService = vehicleService;
        this.physicsService = physicsService;
        this.localAvoidanceService = localAvoidanceService;
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
        headquarter.AvoidanceObstacleId = localAvoidanceService.AddObstacle(prototype.position, prototype.rotation, prototype.config.avoidanceObstaclePrefab);
        headquarter.VehicleObstacleId = vehicleService.RegisterObstacle(prototype.position, prototype.config.vehicleObstaclePrefab);
        headquarter.PhysicsObstacleId = physicsService.RegisterObstacle(prototype.position, prototype.config.physicsObstaclePrefab);
        visuals = GameObject.Instantiate(prototype.config.visualsPrefab, prototype.position, prototype.rotation);
    }

    private void ReadCombatOutput() {
        var combatState = combatSystem.ReadState(headquarter.CombatId);
        if (combatState.damageResult?.damageWasFatal == true) {
            headquarter.Destroyed = true;
            GameObject.Destroy(visuals);
            pathfindingService.UnregisterObstacle(headquarter.PathfindingObstacleId);
            localAvoidanceService.RemoveObstacle(headquarter.AvoidanceObstacleId);
            vehicleService.UnregisterObstacle(headquarter.VehicleObstacleId);
            physicsService.UnregisterObstacle(headquarter.PhysicsObstacleId);
        }
    }

    private void CheckLooseCondition() {
        if (headquarter.Destroyed) {
            Debug.Log("Game over");
        }
    }

}
