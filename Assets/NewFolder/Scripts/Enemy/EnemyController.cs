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
        CaptureSpawnedEnemies();
    }

    private void TryInitEnemySources() {
        if (enemySources.Count == 0) {
            var sourcesFound = SpawnSource.ScanSceneForSources();
            foreach (var source in sourcesFound) {
                var enemySource = new EnemySource();
                enemySource.Origin = source.Position;
                enemySource.SpawnType = source.config.spawnType;
                enemySource.SpawnerId = spawnSystem.AddSpawner(source.Position, source.config);
                enemySource.CurrentCommanderId = commanderSystem.CreateCommander(source.Position);
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
                
                // if (commanderSystem.GetSubordinates(enemySource.CurrentCommanderId) > 30) {
                //     enemySource.CurrentCommanderId = commanderSystem.CreateCommander(enemySource.Origin);
                // }
            } 

            if (enemySource.SpawnType == SpawnType.Armor) {
                foreach (var spawnResult in spawnResultBuffer) {
                    armorAIController.AddAIBehaviour(spawnResult.spawnedId);
                }
            } 
        }
    }

}