using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class CommanderController {

    private readonly PathfindingService pathfindingService;
    private readonly InfantryAIController infantryAIController;
    private readonly ArmorAIController armorAIController;
    private readonly ProducerFactory producerFactory;

    public CommanderController(InfantryAIController squadAIController, ArmorAIController armorAIController, ProducerFactory producerFactory, PathfindingService pathfindingService) {
        this.infantryAIController = squadAIController;
        this.armorAIController = armorAIController;
        this.producerFactory = producerFactory;
        this.pathfindingService = pathfindingService;
    }

    private int idCounter;
    private readonly Dictionary<int, CommanderModel> registry = new();

    public int Create(CommanderPrototype prototype) {
        var nextId = idCounter++;
        var model = new CommanderModel(nextId, prototype.position, prototype.commanderConfig);
        
        model.MainGoalFlowFieldId = pathfindingService.CreateFlowField(Vector3.zero);
        infantryAIController.SetMainGoalFiled(model.MainGoalFlowFieldId);

        var producers = prototype.producerHandles.Select(handle => producerFactory.Create(handle));
        model.Producers.AddRange(producers);
        
        registry[nextId] = model;
        return nextId;
    }

    public void Update() {
        ValidateProducers();
        AssignProducedEntities();
        ReadBehaviorChanges();
    }

    private void ValidateProducers() {
        foreach (var commander in registry.Values) {
            for (int i = commander.Producers.Count - 1; i >= 0; i--) {
                var producer = commander.Producers[i];
                if (!producer.IsValid()) {
                    commander.Producers.RemoveAt(i);
                }
            }
        }
    }

    private void AssignProducedEntities() {
        foreach (var model in registry.Values) {
            foreach (var producer in model.Producers) {
                if (!producer.TryGetSpawnResult(out var spawnResult))
                    continue;

                switch (spawnResult.spawnType) {
                    case SpawnType.Infantry:
                        foreach (var producedInfantry in spawnResult.spawnedIds)
                            infantryAIController.AddInfantryBehavior(producedInfantry, model.Config.infantryAIConfig);
                        break;
                    case SpawnType.Armor:
                        foreach (var producedArmor in spawnResult.spawnedIds)
                            armorAIController.AddAIBehaviour(producedArmor);
                        break;
                    default: 
                        Debug.LogError($"{spawnResult.spawnType}");
                        break;
                }
            }
        }
    }

    private void ReadBehaviorChanges() {
        if (Input.GetKeyDown(KeyCode.R)) {
            foreach (var commander in registry.Values) {
                var switchedStrategyToChaseCenter = !commander.ChasingCenter;
                var targetPosition = switchedStrategyToChaseCenter ? Vector3.zero : commander.Position;
                pathfindingService.UpdateGoal(commander.MainGoalFlowFieldId, targetPosition);
            }
        }
    }
}