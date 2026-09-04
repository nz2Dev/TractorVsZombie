using UnityEngine;

public class EnemyController {

    private readonly SquadsService squadsService;
    private readonly ProductionController productionController;
    private readonly InfantryAIController infantryAIController;
    private readonly ArmorAIController armorAIController;
    private readonly GoalsController goalsController;

    private EnemyModel model;

    public EnemyController(InfantryAIController infantryAIController, ArmorAIController armorAIController, ProductionController productionController, GoalsController goalsController, SquadsService squadsController) {
        this.infantryAIController = infantryAIController;
        this.armorAIController = armorAIController;
        this.productionController = productionController;
        this.goalsController = goalsController;
        this.squadsService = squadsController;
    }

    public void Setup(EnemyPrototype enemyPrototype) {
        model = new EnemyModel(enemyPrototype.infantryAIConfig);

        goalsController.Init(enemyPrototype.goalsPrototype);
        productionController.Init(enemyPrototype.productionPrototype);
        
        infantryAIController.SetMainGoalFiled(goalsController.MainGoalFlowField);
        infantryAIController.SetTargetField(goalsController.TargetFlowField);
    }

    public void Update() {
        if (model == null)
            return;

        productionController.Update();
        if (productionController.IsAnyEntityProduced) {
            foreach (var infantryId in productionController.ProducedInfantries) {
                var formationId = squadsService.AssignToFormation(infantryId);
                infantryAIController.AddInfantryBehavior(infantryId, model.InfantryAIConfig, formationId);
            }
            foreach (var armorId in productionController.ProducedArmors) {
                armorAIController.AddAIBehaviour(armorId);
            }
        }

        goalsController.Update();
        infantryAIController.Update();
        armorAIController.Update();
    }

}