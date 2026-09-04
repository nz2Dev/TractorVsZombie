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
    private int targetFlowFieldId;

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
        ProcessBehaviors();
    }

    public void SetMainGoalFiled(int flowFieldId) {
        mainGoalFlowFieldId = flowFieldId;
    }

    public void SetTargetField(int flowFieldId) {
        targetFlowFieldId = flowFieldId;
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

    private void ProcessBehaviors() {
        foreach (var behaviorModel in models) {
            var infantryId = behaviorModel.InfantryId;
            var infantryState = infantryController.GetInfantryState(infantryId);
            if (!infantryState.isAlive || !infantryState.isGrounded)
                continue;

            if (HasFoeInRange(infantryState, out var foeComponents, out var foePosition)) {
                Attack(infantryId, foeComponents, foePosition);
            } else {
                FollowPath(infantryId, behaviorModel, infantryState, mainGoalFlowFieldId);
            }
        }
    }

    private bool HasFoeInRange(InfantryState infantryState, out EntityComponents foeComponents, out Vector3 foePositon) {
        var foeProximityLayer = CombatSystem.GetProximityLayerForFaction(!infantryState.combatIsAlie);
        if (proximityService.QueryNearestPoint(infantryState.position, foeProximityLayer, out var proximityId)) {
            var point = proximityService.GetPoint(proximityId);
            if (Vector3.Distance(point, infantryState.position) < 5f && entityMapping.TryFindByProximityId(proximityId, out foeComponents)) {
                foePositon = point;
                return true;
            }
        }
        foeComponents = default;
        foePositon = default;
        return false;
    }

    private void Attack(int infantryId, EntityComponents foeComponents, Vector3 foePosition) {
        infantryController.Attack(infantryId, foeComponents.combatId.Value, foePosition);
    }

    private void FollowPath(int infantryId, InfantryAIModel behaviorModel, InfantryState infantryState, int flowFieldId) {
        var flowGoal = pathfindingService.GetGoal(flowFieldId);
        var flowVector = pathfindingService.GetFlowVector(flowFieldId, infantryState.position) * infantryState.maxSpeed;
        var formationForce = formationController.GetFormationForce(behaviorModel.FormationId, infantryState.position);
        var movementVector = Vector3.ClampMagnitude(flowVector + formationForce * behaviorModel.Config.formationBlendFactor, infantryState.maxSpeed);
        infantryController.MoveTo(infantryId, flowGoal, movementVector);
    }

}
