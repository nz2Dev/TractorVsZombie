using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using System.Linq;
using System;
using Codice.Utils;

public class EnemyController {

    private readonly EnemyView enemyView;
    private readonly EnemyConfig enemyConfig;
    private readonly CommanderController commanderController;
    private readonly ProductionBuildingController productionBuildingController;

    public EnemyController(EnemyView crowdView, EnemyConfig enemyConfig, CommanderController waveController, ProductionBuildingController productionBuildingController) {
        this.enemyView = crowdView;
        this.enemyConfig = enemyConfig;
        this.commanderController = waveController;
        this.productionBuildingController = productionBuildingController;
    }

    public void Init() {
        // acts as level bootstraper?
        CreateAllProductionBuildings();
        ActivateAllCommanders();
    }

    public void Update() {
        // todo cap the max units count?
        // set the global goal?
        // are all of that is just the other components of concept of "enemy", not a single main orchestrator?
    }

    private void CreateAllProductionBuildings() {
        var productionBuildingSources = GameObject.FindObjectsByType<ProductionBuildingSource>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var source in productionBuildingSources) {
            productionBuildingController.Create(source.GetPrototype());
        }
    }

    private void ActivateAllCommanders() {
        var commanderSources = GameObject.FindObjectsByType<CommanderSource>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var source in commanderSources) {
            commanderController.Create(source.GetPrototype());
        }
    }

}