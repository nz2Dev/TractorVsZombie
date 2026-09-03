using UnityEngine;

public class EnemyController {

    private readonly InfantryAIController infantryAIController;
    private readonly ArmorAIController armorAIController;
    private readonly ProducerFactory producerFactory;
    private readonly PathfindingService pathfindingService;

    private EnemyModel model;

    public EnemyController(InfantryAIController infantryAIController, ArmorAIController armorAIController, ProducerFactory producerFactory, PathfindingService pathfindingService) {
        this.infantryAIController = infantryAIController;
        this.armorAIController = armorAIController;
        this.producerFactory = producerFactory;
        this.pathfindingService = pathfindingService;
    }

    public void Setup(EnemyPrototype enemyPrototype) {
        model = new EnemyModel(enemyPrototype.infantryAIConfig);

        foreach (var variant in enemyPrototype.producerVariants) {
            var producer = producerFactory.Create(variant);
            producer.SpawnEntity();
            model.Producers.Add(producer);
        }

        model.MainGoalFlowFieldId = pathfindingService.CreateFlowField(Vector3.zero);
        infantryAIController.SetMainGoalFiled(model.MainGoalFlowFieldId);
    }

    public void Update() {
        if (model == null)
            return;

        ValidateProducers();
        AssignProducedEntities();
        ReadBehaviorChanges();
    }

    private void ValidateProducers() {
        for (int i = model.Producers.Count - 1; i >= 0; i--) {
            var producer = model.Producers[i];
            if (!producer.IsValid()) {
                model.Producers.RemoveAt(i);
            }
        }
    }

    private void AssignProducedEntities() {
        foreach (var producer in model.Producers) {
            if (!producer.TryGetSpawnResult(out var spawnResult))
                continue;

            switch (spawnResult.spawnType) {
                case SpawnType.Infantry:
                    foreach (var producedInfantry in spawnResult.spawnedIds)
                        infantryAIController.AddInfantryBehavior(producedInfantry, model.InfantryAIConfig);
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

    private void ReadBehaviorChanges() {
        if (Input.GetKeyDown(KeyCode.R)) {
            var switchedStrategyToChaseCenter = !model.ChasingCenter;
            var targetPosition = switchedStrategyToChaseCenter ? Vector3.zero : new Vector3(10, 0, 0);
            pathfindingService.UpdateGoal(model.MainGoalFlowFieldId, targetPosition);
        }
    }

}