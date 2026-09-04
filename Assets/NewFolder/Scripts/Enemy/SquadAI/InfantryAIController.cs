using System;
using System.Collections.Generic;

using Combat;

using UnityEngine;

public class InfantryAIController {

    private readonly FormationController formationController;
    private readonly InfantryController infantryController;
    private readonly PathfindingService pathfindingService;
    private readonly ProximityService proximityService;
    private readonly EntityMapping entityMapping;

    private readonly List<InfantryAIModel> models = new();
    private int mainGoalFlowFieldId;

    public InfantryAIController(InfantryController infantryController, PathfindingService pathfindingService,
        ProximityService proximityService, EntityMapping entityMapping, FormationController formationController) {
        this.infantryController = infantryController;
        this.pathfindingService = pathfindingService;
        this.proximityService = proximityService;
        this.entityMapping = entityMapping;
        this.formationController = formationController;
    }

    public void Update() {
        ValidateBehaviors();
        ProcessCommands();
    }

    public void SetMainGoalFiled(int flowFieldId) {
        mainGoalFlowFieldId = flowFieldId;
    }

    public void AddInfantryBehavior(int infantryId, InfantryAIConfig config, FormationId formationId) {
        var model = new InfantryAIModel(config, infantryId, formationId);
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
            var flowVector = pathfindingService.GetFlowVector(mainGoalFlowFieldId, infantry.position) * infantry.maxSpeed;
            var formationForce = formationController.GetFormationForce(state.FormationId, infantry.position);
            var movementVector = Vector3.ClampMagnitude(flowVector + formationForce * state.Config.formationBlendFactor, infantry.maxSpeed);
            infantryController.MoveTo(infantryId, flowGoal, movementVector);

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
