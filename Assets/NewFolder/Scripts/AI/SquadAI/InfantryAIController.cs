using System.Collections.Generic;

using Combat;

using UnityEngine;

public class InfantryAIController {

    private readonly InfantryController infantryController;
    private readonly PathfindingService pathfindingService;
    private readonly ProximityService proximityService;
    private readonly EntityMapping entityMapping;

    private readonly List<InfantryAIModel> models = new();
    private int mainGoalFlowFieldId;

    public InfantryAIController(InfantryController infantryController, PathfindingService pathfindingService,
        ProximityService proximityService, EntityMapping entityMapping) {
        this.infantryController = infantryController;
        this.pathfindingService = pathfindingService;
        this.proximityService = proximityService;
        this.entityMapping = entityMapping;
    }

    public void Update() {
        ValidateBehaviors();
        ProcessCommands();
    }

    public void SetMainGoalFiled(int flowFieldId) {
        mainGoalFlowFieldId = flowFieldId;
    }

    public void AddInfantryBehavior(int infantryId, InfantryAIConfig config) {
        var model = new InfantryAIModel(config, infantryId);
        models.Add(model);
    }

    private void ValidateBehaviors() {
        for (int i = models.Count - 1; i >= 0; i--) {
            var behaviorModel = models[i];
            if (!infantryController.IsExist(behaviorModel.InfantryId)) {
                models.RemoveAt(i);
            }
        }
    }

    private void ProcessCommands() {
        foreach (var state in models) {
            var infantryId = state.InfantryId;
            var infantry = infantryController.GetInfantryState(infantryId);
            if (!infantry.isAlive || !infantry.isGrounded)
                continue;

            var flowGoal = pathfindingService.GetGoal(mainGoalFlowFieldId);
            var flowVector = pathfindingService.GetFlowVector(mainGoalFlowFieldId, infantry.position);
            // var formationVector = state.Formation.GetFormationVector(subordinateIndex);
            infantryController.MoveTo(infantryId, flowGoal, flowVector * infantry.maxSpeed);

            var foeProximityLayer = CombatSystem.GetProximityLayerForFaction(!infantry.combatIsAlie);
            if (proximityService.QueryNearestPoint(infantry.position, foeProximityLayer, out var proximityId)) {
                var point = proximityService.GetPoint(proximityId);
                if (Vector3.Distance(point, infantry.position) < 5f && entityMapping.TryFindByProximityId(proximityId, out var components)) {
                    infantryController.Attack(infantryId, components.combatId.Value, point);
                }
            }
        }
    }

}
