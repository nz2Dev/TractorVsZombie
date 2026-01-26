using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using System.Linq;
using System;

public class EnemyController {

    private readonly EnemyView enemyView;
    private readonly MilitaryBuildingController buildingController;
    private readonly CommanderSystem commanderSystem;

    private readonly List<EnemySource> enemySources = new();

    public EnemyController(EnemyView crowdView, MilitaryBuildingController buildingController, CommanderSystem commanderSystem) {
        this.enemyView = crowdView;
        this.buildingController = buildingController;
        this.commanderSystem = commanderSystem;
    }

    public void Update() {
        TryInitEnemySources();
        HireNewCommanders();
        ReadBehaviorChanges();
    }

    private void TryInitEnemySources() {
        if (enemySources.Count == 0) {
            var placesFound = MilitaryBuildingPlace.ScanSceneForPlaces();
            foreach (var place in placesFound) {
                var firstCommander = commanderSystem.CreateCommander();
                var buildingId = buildingController.CreateBuilding(place.Position, place.config, alie: false, firstCommander);
                
                var enemySource = new EnemySource();
                enemySource.Origin = place.Position;
                enemySource.BuildingId = buildingId;
                enemySource.LastCommanderId = firstCommander;
                enemySource.Commanders.Add(firstCommander);
                
                enemySources.Add(enemySource);
            }
        }
    }
    
    private void HireNewCommanders() {
        foreach (var enemySource in enemySources) {
            var lastCommanderSnapshot = commanderSystem.GetCommanderSnapshot(enemySource.LastCommanderId);
            if (lastCommanderSnapshot.subordinateCount > 50) {
                var nextCommander = commanderSystem.CreateCommander();
                enemySource.Commanders.Add(nextCommander);
                enemySource.LastCommanderId = nextCommander;
                buildingController.SetAssignedCommander(enemySource.BuildingId, nextCommander);
            }
        }
    }

    private void ReadBehaviorChanges() {
        if (Input.GetKeyDown(KeyCode.R)) {
            foreach (var enemySource in enemySources) {
                foreach (var commanderId in enemySource.Commanders) {
                    var commanderSnapshot = commanderSystem.GetCommanderSnapshot(commanderId);
                    var switchedStrategyToChaseCenter = !commanderSnapshot.isChasingCenter;
                    var targetPosition = switchedStrategyToChaseCenter ? Vector3.zero : enemySource.Origin;
                    commanderSystem.SetStrategy(commanderId, switchedStrategyToChaseCenter, targetPosition);
                }
            }
        }
    }

}