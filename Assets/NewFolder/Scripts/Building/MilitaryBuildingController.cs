using System.Collections.Generic;
using UnityEngine;

public class MilitaryBuildingController {

    private readonly MilitaryBuildingView view;
    private readonly CombatSystem combatSystem;
    private readonly SpawningService spawningService;
    private readonly BehaviorSystem behaviorSystem;
    private readonly CommanderSystem commanderSystem;
    private readonly ArmorAIController armorAIController;
    
    private int idCounter;
    private readonly Dictionary<int, MilitaryBuildingModel> registry = new();
    private readonly List<int> removalBuffer = new();

    public MilitaryBuildingController(
        CombatSystem combatSystem,
        SpawningService spawningService,
        BehaviorSystem behaviorSystem,
        CommanderSystem commanderSystem,
        ArmorAIController armorAIController,
        MilitaryBuildingView view) {
        this.combatSystem = combatSystem;
        this.spawningService = spawningService;
        this.behaviorSystem = behaviorSystem;
        this.commanderSystem = commanderSystem;
        this.armorAIController = armorAIController;
        this.view = view;
    }

    public int CreateBuilding(Vector3 position, Quaternion rotation, MilitaryBuildingConfig config, bool alie, int commanderId) {
        var id = ++idCounter;
        var model = new MilitaryBuildingModel(id, config);
        
        model.Position = position;
        model.Alie = alie;
        model.CommanderId = commanderId;
        model.CombatId = combatSystem.RegisterAgent(position, alie, model.Config.maxHealth, config.height);
        model.NextSpawnTime = Time.time + config.spawnInterval;
        registry[id] = model;

        view.AddVisuals(model.Id, position, rotation, model.Config.visualsPrefab);
        return id;
    }

    public void Update() {
        ReadCombatOutput();
        ValidateBuildings();
        ProduceSpawns();
    }

    public void SetAssignedCommander(int id, int commanderId) {
        registry[id].CommanderId = commanderId;
    }

    private void ReadCombatOutput() {
        foreach (var model in registry.Values) {
            var combatOutput = combatSystem.GetCombatOutput(model.CombatId);
            if (combatOutput.damageWasFatal) {
                model.Destroyed = true;
            }
        }
    }

    private void ValidateBuildings() {
        removalBuffer.Clear();

        foreach (var model in registry.Values)
            if (model.Destroyed)
                removalBuffer.Add(model.Id);

        foreach (var id in removalBuffer)
            DestroyBuilding(id);
    }

    private void ProduceSpawns() {
        foreach (var model in registry.Values) {
            if (Time.time < model.NextSpawnTime)
                continue;

            model.NextSpawnTime = Time.time + model.Config.spawnInterval;
            if (model.Config.spawnType == SpawnType.Infantry) {
                if (spawningService.TryProduceInfantry(model.Position, model.Alie, model.Config.infantryConfig, out var spawnedId)) {
                    var actorId = behaviorSystem.CreateActor(spawnedId);
                    commanderSystem.AddSubordinate(model.CommanderId, actorId);
                }
            } else if (model.Config.spawnType == SpawnType.Armor) {
                if (spawningService.TryProduceArmor(model.Position, model.Config.armorConfig, out var spawnedId)) {
                    armorAIController.AddAIBehaviour(spawnedId);
                }
            }
        }
    }

    private void DestroyBuilding(int id) {
        registry.Remove(id, out var model);
        combatSystem.UnregisterAgent(model.CombatId);
        view.RemoveVisuals(model.Id);
    }
}
