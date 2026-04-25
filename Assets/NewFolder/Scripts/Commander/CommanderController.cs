using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class CommanderController {

    private readonly SquadAIController squadAIController;
    private readonly ArmorAIController armorAIController;
    private readonly ProducerFactory producerFactory;

    public CommanderController(
        SquadAIController squadAIController, ArmorAIController armorAIController,
        ProducerFactory producerFactory) {
        this.squadAIController = squadAIController;
        this.armorAIController = armorAIController;
        this.producerFactory = producerFactory;
    }

    private int idCounter;
    private readonly Dictionary<int, CommanderModel> registry = new();

    public int Create(CommanderPrototype prototype) {
        var nextId = idCounter++;
        var model = new CommanderModel(nextId, prototype.position, prototype.commanderConfig);
        
        model.LastSquadId = squadAIController.CreateSquad(model.Config.squadAIConfig);
        var producers = prototype.producerHandles.Select(handle => producerFactory.Create(handle));
        model.Producers.AddRange(producers);
        
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
                            squadAIController.AddSubordinate(model.LastSquadId, producedInfantry);
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