using System;
using System.Collections.Generic;


using UnityEngine;

public class InfantryAIController {

    private readonly FormationController formationController;
    private readonly InfantryController infantryController;
    private readonly PathfindingService pathfindingService;
    private readonly HitboxSystem hitboxSystem;
    private readonly EntityMapping entityMapping;

    private readonly List<InfantryAIModel> models = new();
    private int mainGoalFlowFieldId;
    private int targetFlowFieldId;

    public InfantryAIController(InfantryController infantryController, PathfindingService pathfindingService,
        HitboxSystem hitboxSystem, EntityMapping entityMapping, FormationController formationController) {
        this.infantryController = infantryController;
        this.pathfindingService = pathfindingService;
        this.hitboxSystem = hitboxSystem;
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

            if (!TryGetClosestFoe(infantryState, out var foeHitboxId, out var foePosition, out var foeHitboxState))
                continue;

            var distanceToAttack = Vector3.Distance(infantryState.position, foePosition);
            var maxAttackDistance = infantryState.activationRadius + foeHitboxState.radius;
            if (infantryState.attackActivated && distanceToAttack > maxAttackDistance) {
                infantryController.StopAttack(infantryId);
            }

            if (!infantryState.attackActivated && distanceToAttack < maxAttackDistance * 0.7f) {
                infantryController.Attack(infantryId, foeHitboxId);
            } else if (IsPathGoalInCostRange(targetFlowFieldId, behaviorModel.Config.targetAgroCostRange, infantryState.position)) {
                FollowPath(infantryId, behaviorModel, infantryState, targetFlowFieldId);
            }  else {
                FollowPath(infantryId, behaviorModel, infantryState, mainGoalFlowFieldId);
            }
        }
    }

    private bool IsPathGoalInCostRange(int flowFieldId, float costRange, Vector3 position) {
        return pathfindingService.GetFlowCost(flowFieldId, position) < costRange;
    }

    private bool TryGetClosestFoe(InfantryState infantryState, out HitboxId foeHitboxId, out Vector3 foePosition, out HitboxState foeHitboxState) {
        if (hitboxSystem.QueryNearest(infantryState.position, !infantryState.factionIsAlie, out foeHitboxId)) {
            if (entityMapping.TryFindByHitboxId(foeHitboxId, out _)) {
                foeHitboxState = hitboxSystem.ReadState(foeHitboxId);
                foePosition = hitboxSystem.GetPosition(foeHitboxId);
                return true;
            }
        }
        foeHitboxId = default;
        foePosition = default;
        foeHitboxState = default;
        return false;
    }

    private void FollowPath(int infantryId, InfantryAIModel behaviorModel, InfantryState infantryState, int flowFieldId) {
        var flowVector = pathfindingService.GetFlowVector(flowFieldId, infantryState.position) * infantryState.maxSpeed;
        var formationForce = formationController.GetFormationForce(behaviorModel.FormationId, infantryState.position);
        var movementVector = Vector3.ClampMagnitude(flowVector + formationForce * behaviorModel.Config.formationBlendFactor, infantryState.maxSpeed);
        infantryController.MoveTo(infantryId, infantryState.position + movementVector);
    }

}
