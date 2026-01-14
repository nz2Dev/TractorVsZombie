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
    private readonly InfantryAIController infantryAIController;

    private List<int> spawners = new();
    private List<SpawnResult> spawnResultBuffer = new(32);
    private Transform targetPoint;

    public EnemyController(
        EnemyView crowdView,
        SpawnSystem spawnSystem,
        Transform targetPoint, 
        ArmorAIController armorAIController, 
        InfantryAIController infantryAIController) {
        
        this.enemyView = crowdView;
        this.spawnSystem = spawnSystem;
        this.targetPoint = targetPoint;
        this.armorAIController = armorAIController;
        this.infantryAIController = infantryAIController;
    }

    public void Update() {
        ScanSpawnSources();
        AssignSpawnedAI();
        UpdateAI();
    }

    private void ScanSpawnSources() {
        if (spawners.Count == 0) {
            infantryAIController.InitFormations();
            var sourcesFound = SpawnSource.ScanSceneForSources();
            foreach (var source in sourcesFound) {
                var spawnerId = spawnSystem.AddSpawner(source.Position, source.config);
                spawners.Add(spawnerId);
            }
        }
    }

    private void AssignSpawnedAI() {
        spawnSystem.CaptureCompletedSpawns(spawners, spawnResultBuffer);
        foreach (var spawnResult in spawnResultBuffer) {
            if (spawnResult.spawnType == SpawnType.Infantry) {
                infantryAIController.TakeUnderControl(spawnResult.spawnedId);
            } else if (spawnResult.spawnType == SpawnType.Armor) {
                armorAIController.TakeUnderControl(spawnResult.spawnedId);
            } else {
                throw new Exception();
            }
        }
    }

    private void UpdateAI() {
        armorAIController.SetGoal(targetPoint.position);
        infantryAIController.SetGoal(targetPoint.position);
    }

}