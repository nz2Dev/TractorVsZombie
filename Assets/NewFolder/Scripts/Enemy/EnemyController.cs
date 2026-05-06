using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using System.Linq;
using System;
using Codice.Utils;

public class EnemyController {

    private readonly CommanderController commanderController;
    private readonly ProductionBuildingController productionBuildingController;
    private readonly ProductionSpaceController productionSpaceController;

    public EnemyController(CommanderController commanderController, ProductionBuildingController productionBuildingController, ProductionSpaceController productionSpaceController) {
        this.commanderController = commanderController;
        this.productionBuildingController = productionBuildingController;
        this.productionSpaceController = productionSpaceController;
    }

    public void Setup(EnemyPrototype enemyPrototype) {
        foreach (var productionBuildingPrototype in enemyPrototype.productionBuildingPrototypes)
            productionBuildingController.Create(productionBuildingPrototype);
        
        foreach (var productionSpacePrototype in enemyPrototype.productionSpacePrototypes)
            productionSpaceController.Create(productionSpacePrototype);

        foreach (var commanderPrototype in enemyPrototype.commanderPrototypes)
            commanderController.Create(commanderPrototype);
    }

    public void Update() {
        // todo cap the max units count?
        // set the global goal?
        // are all of that is just the other components of concept of "enemy", not a single main orchestrator?
    }

}