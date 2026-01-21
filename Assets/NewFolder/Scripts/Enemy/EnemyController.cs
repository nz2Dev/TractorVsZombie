using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using System.Linq;
using UnityEngine.Assertions;
using System;

public class EnemyController {

    private readonly EnemyView enemyView;
    private readonly SpawnSystem spawnSystem;
    private readonly ArmorAIController armorAIController;
    private readonly CommanderSystem commanderSystem;

    private readonly List<EnemySource> enemySources = new();
    private readonly List<SpawnResult> spawnResultBuffer = new(32);

    public EnemyController(EnemyView crowdView, SpawnSystem spawnSystem, ArmorAIController armorAIController, CommanderSystem commanderSystem) {
        this.enemyView = crowdView;
        this.spawnSystem = spawnSystem;
        this.armorAIController = armorAIController;
        this.commanderSystem = commanderSystem;
    }

    public void Update() {
        TryInitEnemySources();
        HireNewCommanders();
        CaptureSpawnedEnemies();
        ReadBehaviorChanges();
    }

    private void TryInitEnemySources() {
        if (enemySources.Count == 0) {
            var sourcesFound = SpawnSource.ScanSceneForSources();
            foreach (var source in sourcesFound) {
                var enemySource = new EnemySource();
                enemySource.Origin = source.Position;
                enemySource.SpawnType = source.config.spawnType;
                enemySource.SpawnerId = spawnSystem.AddSpawner(source.Position, source.config);
                var firstCommander = commanderSystem.CreateCommander();
                enemySource.Commanders.Add(firstCommander);
                enemySource.LastCommanderId = firstCommander;
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
            }
        }
    }

    private void CaptureSpawnedEnemies() {
        foreach (var enemySource in enemySources) {
            spawnSystem.GetCompletedSpawns(enemySource.SpawnerId, spawnResultBuffer);
            
            if (enemySource.SpawnType == SpawnType.Infantry) {
                foreach (var spawnResult in spawnResultBuffer) {
                    commanderSystem.AddSubordinate(enemySource.LastCommanderId, spawnResult.spawnedId);
                }
            } 

            if (enemySource.SpawnType == SpawnType.Armor) {
                foreach (var spawnResult in spawnResultBuffer) {
                    armorAIController.AddAIBehaviour(spawnResult.spawnedId);
                }
            } 
        }
    }

    private void ReadBehaviorChanges() {
        if (Input.GetKeyDown(KeyCode.R)) {
            foreach (var enemySource in enemySources) {
                foreach (var commanderId in enemySource.Commanders) {
                    var commanderSnapshot = commanderSystem.GetCommanderSnapshot(commanderId);
                    var switchedStrategyToChaseCenter = commanderSnapshot.isChasingCenter ? false : true;
                    var targetPosition = switchedStrategyToChaseCenter ? Vector3.zero : enemySource.Origin;
                    commanderSystem.SetStrategy(commanderId, switchedStrategyToChaseCenter, targetPosition);
                }
            }
        }
    }

}