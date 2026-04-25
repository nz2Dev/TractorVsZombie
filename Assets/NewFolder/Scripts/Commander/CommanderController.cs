using System;
using System.Collections.Generic;

using UnityEngine;

public class CommanderController {

    private readonly SquadAIController squadAIController;
    private readonly ArmorAIController armorAIController;
    private readonly ProductionBuildingController productionBuildingController;

    public CommanderController(
        SquadAIController squadAIController, ArmorAIController armorAIController, 
        ProductionBuildingController productionBuildingController
        ) {
        this.squadAIController = squadAIController;
        this.armorAIController = armorAIController;
        this.productionBuildingController = productionBuildingController;
    }

    private int idCounter;
    private readonly Dictionary<int, CommanderModel> registry = new();

    public int Create(CommanderPrototype prototype) {
        var nextId = idCounter++;
        var model = new CommanderModel(nextId, prototype.position, prototype.commanderConfig);
        model.ProductionBuildingIds.Add(prototype.productionBuildingId);
        model.LastSquadId = squadAIController.CreateSquad(model.Config.squadAIConfig);;
        registry[nextId] = model;
        return nextId;
    }

    public void Update() {
        MakeNewSquads();
        ValidateProducers();
        AssignProducedEntities();
        ReadBehaviorChanges();
    }

    private void MakeNewSquads() {
        foreach (var model in registry.Values) {
            var lastSquadSnapshot = squadAIController.GetSquadSnapshot(model.LastSquadId);
            if (lastSquadSnapshot.subordinateCount > 50) {
                var nextSquadId = squadAIController.CreateSquad(model.Config.squadAIConfig);
                model.SquadIds.Add(nextSquadId);
                model.LastSquadId = nextSquadId;
            }
        }
    }

    private void ValidateProducers() {
        foreach (var commander in registry.Values) {
            for (int i = commander.ProductionBuildingIds.Count - 1; i >= 0; i--) {
                var productionBuildingId = commander.ProductionBuildingIds[i];
                if (!productionBuildingController.IsExist(productionBuildingId)) {
                    commander.ProductionBuildingIds.RemoveAt(i);
                }
            }
        }
    }

    private void AssignProducedEntities() {
        foreach (var model in registry.Values) {
            foreach (var productionBuildingId in model.ProductionBuildingIds) {
                var buildingSpawnResult = productionBuildingController.ReadState(productionBuildingId).lastResult;
                if (buildingSpawnResult == null)
                    continue;

                switch (buildingSpawnResult.spawnType) {
                    case SpawnType.Infantry:
                        foreach (var producedInfantry in buildingSpawnResult.spawnedIds)
                            squadAIController.AddSubordinate(model.LastSquadId, producedInfantry);
                        break;
                    case SpawnType.Armor:
                        foreach (var producedArmor in buildingSpawnResult.spawnedIds)
                            armorAIController.AddAIBehaviour(producedArmor);
                        break;
                    default: 
                        Debug.LogError($"{buildingSpawnResult.spawnType}");
                        break;
                }
            }
        }
    }

    private void ReadBehaviorChanges() {
        if (Input.GetKeyDown(KeyCode.R)) {
            foreach (var commander in registry.Values) {
                foreach (var squadId in commander.SquadIds) {
                    var snapshot = squadAIController.GetSquadSnapshot(squadId);
                    var switchedStrategyToChaseCenter = !snapshot.isChasingCenter;
                    var targetPosition = switchedStrategyToChaseCenter ? Vector3.zero : commander.Position;
                    squadAIController.SetStrategy(squadId, switchedStrategyToChaseCenter, targetPosition);
                }
            }
        }
    }
}