using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using System.Linq;
using UnityEngine.Assertions;

public class EnemyController {

    private readonly WeaponController weaponController;
    private readonly VehicleController vehicleController;
    private readonly InfantryController infantryController;

    private readonly EnemyView enemyView;
    private readonly NavigationService navigationService;
    private readonly CombatService combatService;

    private float lastTimeProduced = float.MinValue;
    private Transform[] spawnPoints;
    private Transform targetPoint;
    private int unitsCount;
    private readonly InfantryConfig infantryConfig;
    private readonly List<int> infantryIds = new List<int>();


    private float lastTimeProducedVehicle;
    private Transform[] vehiclesSpawnPoints;
    private int maxVehiclesCount;
    private int lastSpawnIndex;

    private readonly VehicleConfig vehicleConfig;
    private readonly WeaponConfig vehicleWeaponConfig;
    private readonly List<EnemyVehicleModel> vehicleModels = new ();

    private readonly List<EnemyVehicleModel> diedVehicles = new ();

    public EnemyController(EnemyView crowdView, NavigationService navigationService, Transform[] spawnPoints, Transform targetPoint, int unitsCount,
        CombatService combatService, int maxVehiclesCount, WeaponController weaponController,
        VehicleController vehicleController, VehicleConfig vehicleConfig, WeaponConfig vehicleWeaponConfig, InfantryConfig infantryConfig, InfantryController infantryController) {
        this.enemyView = crowdView;
        this.navigationService = navigationService;
        this.spawnPoints = spawnPoints;
        this.targetPoint = targetPoint;
        this.unitsCount = unitsCount;
        this.combatService = combatService;

        this.vehiclesSpawnPoints = spawnPoints; // reusing
        this.maxVehiclesCount = maxVehiclesCount;

        this.weaponController = weaponController;
        this.vehicleController = vehicleController;
        this.vehicleConfig = vehicleConfig;
        this.vehicleWeaponConfig = vehicleWeaponConfig;
        this.infantryConfig = infantryConfig;
        this.infantryController = infantryController;
    }

    public IReadOnlyList<EnemyVehicleModel> GetDiedVehicles() {
        return diedVehicles;
    }

    public void Init() {
        navigationService.SetGoal(targetPoint.position);
    }

    private void ProduceNewUnits() {
        if (infantryIds.Count > unitsCount) 
            return;

        if (lastTimeProduced + 0.1f > Time.time)
            return;
        
        lastTimeProduced = Time.time;
        foreach (var spawnPoint in spawnPoints) {    
            SpawnInfantry(spawnPoint.position);
        }
    }

    private void ProduceNewVehicleUnits() {
        if (vehicleModels.Count > maxVehiclesCount)
            return;

        if (lastTimeProducedVehicle + 1f > Time.time)
            return;

        lastTimeProducedVehicle = Time.time;
        var nextSpawnIndex = lastSpawnIndex++ % vehiclesSpawnPoints.Length;
        SpawnVehicle(vehiclesSpawnPoints[nextSpawnIndex].position);
    }

    private void SpawnInfantry(Vector3 position) {
        var infantryId = infantryController.SpawnInfantry(position, alie: false, infantryConfig);
        infantryIds.Add(infantryId);
    }

    public void Update() {
        UpdateGoal();
        ProduceNewUnits();

        ProduceNewVehicleUnits();
        UpdateVehicleNavigation();
        SyncVehiclesPositions();
        ReadVehiclesCombat();
        FilterDeadVehicles();
    }

    private void UpdateGoal() {
        var tractorPosition = vehicleController.GetVehiclePosition(1);
        targetPoint.position = tractorPosition;
        navigationService.SetGoal(tractorPosition);
    }

    private void SpawnVehicle(Vector3 position) {
        var combatAgentId = combatService.RegisterAgent(position, alie: false);
        var model = new EnemyVehicleModel {
            Health = 5,
            Position = position,
            VehicleId = vehicleController.SpawnVehicle(position, vehicleConfig),
            CombatId = combatAgentId,
            WeaponId = weaponController.SpawnWeapon(combatAgentId, position, vehicleWeaponConfig)
        };
        vehicleModels.Add(model);
    }

    private void DespawnVehicleAt(int vehicleIndex) {
        var model = vehicleModels[vehicleIndex];
        vehicleController.DeleteVehicle(model.VehicleId);
        combatService.UnregisterAgent(model.CombatId);
        weaponController.DeleteWeapon(model.WeaponId);
        vehicleModels.RemoveAt(vehicleIndex);
    }

    private void SyncVehiclesPositions() {
        foreach (var model in vehicleModels) {
            model.Position = vehicleController.GetVehiclePosition(model.VehicleId);
            weaponController.MoveWeapon(model.WeaponId, model.Position);
            combatService.UpdateAgentPosition(model.CombatId, model.Position);
        }
    }

    private void ReadVehiclesCombat() {
        foreach (var model in vehicleModels) {
            var vehicleCombatState = combatService.GetAgentState(model.CombatId);
            
            if (vehicleCombatState.projectiled || vehicleCombatState.exploded) {    
                model.Health -= vehicleCombatState.damage;
            }

            combatService.ClearAgentState(model.CombatId);

            if (model.Health <= 0) {
                diedVehicles.Add(model);
            }
        }
    }

    private void FilterDeadVehicles() {
        for (int i = 0; i < vehicleModels.Count; i++) {
            var model = vehicleModels[i];
            if (model.Health <= 0) {
                DespawnVehicleAt(i);
                i--;
            }
        }
    }

    private void UpdateVehicleNavigation() {
        foreach (var model in vehicleModels) {
            var distance = Vector3.Distance(targetPoint.position, model.Position);

            var gasDistance = 10;
            var gas = Mathf.Floor(Mathf.Clamp(distance, 0, gasDistance) / gasDistance);
            vehicleController.DriveVehicle(model.VehicleId, gas, false);
            
            var stopDistance = 5f;
            var brakes = 1 - Mathf.Floor(Mathf.Clamp(distance, 0, stopDistance) / stopDistance);
            vehicleController.BrakeVehicle(model.VehicleId, brakes);

            var flowVector = navigationService.GetFlowVector(model.Position);
            vehicleController.SteerVehicleToward(model.VehicleId, flowVector);
        }
    }

}