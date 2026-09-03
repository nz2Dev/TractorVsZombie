using UnityEngine;

public class EnemyController {

    private readonly ProductionController productionController;
    private readonly InfantryAIController infantryAIController;
    private readonly ArmorAIController armorAIController;
    private readonly PathfindingService pathfindingService;

    private EnemyModel model;

    public EnemyController(InfantryAIController infantryAIController, ArmorAIController armorAIController, ProductionController productionController, PathfindingService pathfindingService) {
        this.infantryAIController = infantryAIController;
        this.armorAIController = armorAIController;
        this.productionController = productionController;
        this.pathfindingService = pathfindingService;
    }

    public void Setup(EnemyPrototype enemyPrototype) {
        model = new EnemyModel(enemyPrototype.infantryAIConfig);

        productionController.Init(enemyPrototype.productionPrototype);

        model.MainGoalFlowFieldId = pathfindingService.CreateFlowField(Vector3.zero);
        infantryAIController.SetMainGoalFiled(model.MainGoalFlowFieldId);
    }

    public void Update() {
        if (model == null)
            return;

        productionController.Update();
        if (productionController.IsAnyEntityProduced) {
            foreach (var infantryId in productionController.ProducedInfantries) {
                infantryAIController.AddInfantryBehavior(infantryId, model.InfantryAIConfig);
            }
            foreach (var armorId in productionController.ProducedArmors) {
                armorAIController.AddAIBehaviour(armorId);
            }
        }

        infantryAIController.Update();
        armorAIController.Update();
        
        ReadBehaviorChanges();
    }

    private void ReadBehaviorChanges() {
        if (Input.GetKeyDown(KeyCode.R)) {
            var switchedStrategyToChaseCenter = !model.ChasingCenter;
            var targetPosition = switchedStrategyToChaseCenter ? Vector3.zero : new Vector3(10, 0, 0);
            pathfindingService.UpdateGoal(model.MainGoalFlowFieldId, targetPosition);
        }
    }

}