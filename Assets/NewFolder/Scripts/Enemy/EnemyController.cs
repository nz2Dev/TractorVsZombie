using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using System.Linq;
using System;
using Codice.Utils;

public class EnemyController {

    private readonly EnemyView enemyView;
    private readonly EnemyConfig enemyConfig;
    private readonly ProductionBuildingController productionBuildingController;
    private readonly SquadAIController squadAIController;
    private readonly ArmorAIController armorAIController;

    private readonly List<EnemySource> enemySources = new();

    public EnemyController(EnemyView crowdView, EnemyConfig enemyConfig, ProductionBuildingController buildingController, SquadAIController commanderSystem, ArmorAIController armorAIController) {
        this.enemyView = crowdView;
        this.enemyConfig = enemyConfig;
        this.productionBuildingController = buildingController;
        this.squadAIController = commanderSystem;
        this.armorAIController = armorAIController;
    }

    public void Update() {
        TryInitEnemySources();
        ValidateEnemySource();
        MakeNewSquads();
        AssignProducedEnemies();
        UpdateProductionQueues();
        ReadBehaviorChanges();
    }

    private void TryInitEnemySources() {
        if (enemySources.Count != 0)
            return;
        
        var productionBuildingPlaces = GameObject.FindObjectsByType<BuildingPlace>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(place => place.configType == BuildingConfigType.ProductionBuilding);

        foreach (var place in productionBuildingPlaces) {
            var enemySource = new EnemySource();
            enemySource.Origin = place.Position;
            enemySource.ProductionBuildingId = productionBuildingController.Spawn(place.Position, place.Rotation, place.productionBuildingConfig, alie: false);
            enemySource.ProductionBuildingConfig = place.productionBuildingConfig;
            
            var firstSquadId = squadAIController.CreateSquad();
            enemySource.LastSquadId = firstSquadId;
            enemySource.SquadIds.Add(firstSquadId);

            enemySources.Add(enemySource);
        }
    }

    private void ValidateEnemySource() {
        for (int i = enemySources.Count - 1; i >= 0; i--) {
            var enemySource = enemySources[i];
            if (!productionBuildingController.IsExist(enemySource.ProductionBuildingId)) {
                enemySources.RemoveAt(i);
            }
        }
    }

    private void MakeNewSquads() {
        foreach (var enemySource in enemySources) {
            var lastSquadSnapshot = squadAIController.GetSquadSnapshot(enemySource.LastSquadId);
            if (lastSquadSnapshot.subordinateCount > 50) {
                var nextSquadId = squadAIController.CreateSquad();
                enemySource.SquadIds.Add(nextSquadId);
                enemySource.LastSquadId = nextSquadId;
            }
        }
    }

    private void AssignProducedEnemies() {
        foreach (var enemySource in enemySources) {
            var productionResult = productionBuildingController.ReadProductionResult(enemySource.ProductionBuildingId);
            switch (productionResult.spawnType) {
                case SpawnType.Infantry:
                    foreach (var producedInfantry in productionResult.spawnedIds)
                        squadAIController.AddSubordinate(enemySource.LastSquadId, producedInfantry);
                    break;
                case SpawnType.Armor:
                    foreach (var producedArmor in productionResult.spawnedIds)
                        armorAIController.AddAIBehaviour(producedArmor);
                    break;
                default: 
                    Debug.LogError($"{productionResult.spawnType}");
                    break;
            }
        }
    }

    private void UpdateProductionQueues() {
        var infantrySources = Mathf.Max(enemySources.Count(source => source.ProductionBuildingConfig.spawnType == SpawnType.Infantry), 1);
        var availableInfantryQueue = enemyConfig.maxInfantryCount - productionBuildingController.GetProductionLoad(SpawnType.Infantry);
        var armorSources = Mathf.Max(enemySources.Count(source => source.ProductionBuildingConfig.spawnType == SpawnType.Armor), 1);
        var availableArmorQueue = enemyConfig.maxArmorCount - productionBuildingController.GetProductionLoad(SpawnType.Armor);

        Debug.Log($"{availableInfantryQueue}");

        foreach (var enemySource in enemySources) {
            switch (enemySource.ProductionBuildingConfig.spawnType) {
                case SpawnType.Infantry:
                    productionBuildingController.SetQueueAmount(enemySource.ProductionBuildingId, 
                        availableInfantryQueue / infantrySources);
                    break;
                case SpawnType.Armor:
                    productionBuildingController.SetQueueAmount(enemySource.ProductionBuildingId,
                        availableArmorQueue / armorSources);
                    break;
                default:
                    Debug.LogError($"{enemySource.ProductionBuildingConfig.spawnType}");
                    break;
            }
        }
    }

    private void ReadBehaviorChanges() {
        if (Input.GetKeyDown(KeyCode.R)) {
            foreach (var enemySource in enemySources) {
                foreach (var squadId in enemySource.SquadIds) {
                    var snapshot = squadAIController.GetSquadSnapshot(squadId);
                    var switchedStrategyToChaseCenter = !snapshot.isChasingCenter;
                    var targetPosition = switchedStrategyToChaseCenter ? Vector3.zero : enemySource.Origin;
                    squadAIController.SetStrategy(squadId, switchedStrategyToChaseCenter, targetPosition);
                }
            }
        }
    }

}