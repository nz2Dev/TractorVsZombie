using System.Collections.Generic;
using UnityEngine;

public class MilitaryBuildingController {
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
        ArmorAIController armorAIController
    ) {
        this.combatSystem = combatSystem;
        this.spawningService = spawningService;
        this.behaviorSystem = behaviorSystem;
        this.commanderSystem = commanderSystem;
        this.armorAIController = armorAIController;
    }

    public int CreateBuilding(Vector3 position, MilitaryBuildingConfig config, bool alie, int commanderId) {
        var id = ++idCounter;
        var model = new MilitaryBuildingModel(id, config);
        
        model.Position = position;
        model.Health = config.maxHealth;
        model.Alie = alie;
        model.CommanderId = commanderId;
        model.CombatId = combatSystem.RegisterAgent(position, alie, config.height);
        model.NextSpawnTime = Time.time + config.spawnInterval;
        
        registry[id] = model;
        return id;
    }

    public void Update() {
        ReadCombatState();
        ValidateBuildings();
        ProduceSpawns();
    }

    public void SetAssignedCommander(int id, int commanderId) {
        registry[id].CommanderId = commanderId;
    }

    private void ReadCombatState() {
        foreach (var model in registry.Values) {
            var combatState = combatSystem.GetAgentState(model.CombatId);
            
            if (combatState.damage <= 0)
                continue;

            model.Health -= combatState.damage;
            combatSystem.ClearAgentState(model.CombatId);
        }
    }

    private void ValidateBuildings() {
        removalBuffer.Clear();

        foreach (var model in registry.Values)
            if (!model.IsAlive)
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
        var model = registry[id];
        combatSystem.UnregisterAgent(model.CombatId);
        registry.Remove(id);
    }
}
