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
    private readonly NavigationSystem navigationSystem;

    private readonly List<EnemySource> enemySources = new();
    private readonly List<SpawnResult> spawnResultBuffer = new(32);
    private readonly Transform targetPoint;

    public EnemyController(EnemyView crowdView, SpawnSystem spawnSystem, Transform targetPoint, ArmorAIController armorAIController, 
        CommanderSystem commanderSystem, NavigationSystem navigationSystem) {
        this.enemyView = crowdView;
        this.spawnSystem = spawnSystem;
        this.targetPoint = targetPoint;
        this.armorAIController = armorAIController;
        this.commanderSystem = commanderSystem;
        this.navigationSystem = navigationSystem;
    }

    public void Update() {
        TryInitEnemySources();
        CaptureSpawnedEnemies();
        UpdateLeakedNavigationSystemGoal();
    }

    private void TryInitEnemySources() {
        if (enemySources.Count == 0) {
            var sourcesFound = SpawnSource.ScanSceneForSources();
            foreach (var source in sourcesFound) {
                var enemySource = new EnemySource();
                enemySource.SpawnType = source.config.spawnType;
                enemySource.SpawnerId = spawnSystem.AddSpawner(source.Position, source.config);
                enemySource.CurrentCommanderId = commanderSystem.CreateCommander();
                enemySources.Add(enemySource);
            }
        }
    }

    private void CaptureSpawnedEnemies() {
        foreach (var enemySource in enemySources) {
            spawnSystem.GetCompletedSpawns(enemySource.SpawnerId, spawnResultBuffer);
            
            if (enemySource.SpawnType == SpawnType.Infantry) {
                foreach (var spawnResult in spawnResultBuffer) {
                    commanderSystem.AddSubordinate(enemySource.CurrentCommanderId, spawnResult.spawnedId);
                }
                
                if (commanderSystem.GetSubordinates(enemySource.CurrentCommanderId) > 30) {
                    enemySource.CurrentCommanderId = commanderSystem.CreateCommander();
                }
            } 

            if (enemySource.SpawnType == SpawnType.Armor) {
                foreach (var spawnResult in spawnResultBuffer) {
                    armorAIController.AddAIBehaviour(spawnResult.spawnedId);
                }
            } 
        }
    }

    private void UpdateLeakedNavigationSystemGoal() {
        navigationSystem.SetGoal(targetPoint.position);
    }

}