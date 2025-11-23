using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using System.Linq;
using UnityEngine.Assertions;
using System;

public class EnemyController {

    private readonly EnemyView enemyView;
    private readonly InfantryController infantryController;
    private readonly ArmorController armorController;
    private readonly NavigationService navigationService;
    private readonly VehicleController vehicleController;

    private float lastTimeProduced = float.MinValue;
    private Transform[] spawnPoints;
    private Transform targetPoint;
    private int maxInfantryCount;
    private readonly InfantryConfig infantryConfig;

    private float lastTimeProducedVehicle;
    private Transform[] vehiclesSpawnPoints;
    private int maxArmorCount;
    private int lastSpawnIndex;
    private readonly ArmorConfig armorConfig;

    public EnemyController(EnemyView crowdView, NavigationService navigationService,
        Transform[] spawnPoints, Transform targetPoint, int unitsCount, InfantryConfig infantryConfig, InfantryController infantryController,
        int maxVehiclesCount, ArmorConfig armorConfig, ArmorController armorController, VehicleController vehicleController) {
        this.enemyView = crowdView;
        this.navigationService = navigationService;
        this.spawnPoints = spawnPoints;
        this.targetPoint = targetPoint;
        this.maxInfantryCount = unitsCount;
        this.infantryConfig = infantryConfig;
        this.infantryController = infantryController;

        this.vehiclesSpawnPoints = spawnPoints; // reusing
        this.maxArmorCount = maxVehiclesCount;
        this.armorConfig = armorConfig;
        this.armorController = armorController;
        this.vehicleController = vehicleController;
    }

    public void Init() {
        navigationService.SetGoal(targetPoint.position);
    }

    public void Update() {
        UpdateGoal();
        ProduceNewInfantry();
        ProduceNewArmor();
    }

    private void ProduceNewInfantry() {
        if (infantryController.InfantryCount > maxInfantryCount) 
            return;

        if (lastTimeProduced + 0.1f > Time.time)
            return;
        
        lastTimeProduced = Time.time;
        foreach (var spawnPoint in spawnPoints) {    
            infantryController.SpawnInfantry(spawnPoint.position, alie: false, infantryConfig);
        }
    }

    private void ProduceNewArmor() {
        if (armorController.ArmorCount > maxArmorCount)
            return;

        if (lastTimeProducedVehicle + 1f > Time.time)
            return;

        lastTimeProducedVehicle = Time.time;
        var nextSpawnIndex = lastSpawnIndex++ % vehiclesSpawnPoints.Length;
        var nextSpawnPoint = vehiclesSpawnPoints[nextSpawnIndex].position;
        armorController.SpawnArmor(nextSpawnPoint, armorConfig);
    }

    private void UpdateGoal() {
        var tractorPosition = vehicleController.GetVehiclePosition(1);
        targetPoint.position = tractorPosition;
        navigationService.SetGoal(tractorPosition);
    }

}