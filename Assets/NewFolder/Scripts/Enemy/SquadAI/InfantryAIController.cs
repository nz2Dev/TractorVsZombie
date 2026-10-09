using System;
using System.Collections.Generic;

using Combat;

using UnityEngine;

public class InfantryAIController {

    private readonly FormationController formationController;
    private readonly InfantryController infantryController;
    private readonly PathfindingService pathfindingService;
    private readonly ProximityService proximityService;
    private readonly RaycastService raycastService;
    private readonly EntityMapping entityMapping;

    private readonly List<InfantryAIModel> models = new();
    private int mainGoalFlowFieldId;
    private int targetFlowFieldId;

    public InfantryAIController(InfantryController infantryController, PathfindingService pathfindingService,
        ProximityService proximityService, EntityMapping entityMapping, FormationController formationController, RaycastService raycastService) {
        this.infantryController = infantryController;
        this.pathfindingService = pathfindingService;
        this.proximityService = proximityService;
        this.entityMapping = entityMapping;
        this.formationController = formationController;
        this.raycastService = raycastService;
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

            if (!TryGetClosestFoe(infantryState, out var foeProximityId, out var foePosition, out var foeRaycastState))
                continue;

            var distanceToAttack = Vector3.Distance(infantryState.position, foePosition);
            var maxAttackDistance = infantryState.activationRadius + foeRaycastState.radius;
            if (infantryState.attackActivated && distanceToAttack > maxAttackDistance) {
                infantryController.StopAttack(infantryId);
            }

            if (!infantryState.attackActivated && distanceToAttack < maxAttackDistance * 0.7f) {
                infantryController.Attack(infantryId, foeProximityId);
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

    private bool TryGetClosestFoe(InfantryState infantryState, out ProximityId foeProximityId, out Vector3 foePosition, out RaycastState foeRaycastState) {
        var foeProximityLayer = CombatSystem.GetProximityLayerForFaction(!infantryState.combatIsAlie);
        if (proximityService.QueryNearestPoint(infantryState.position, foeProximityLayer, out foeProximityId)) {
            if (entityMapping.TryFindByProximityId(foeProximityId, out var foeComponents)) {
                foeRaycastState = raycastService.ReadState(foeComponents.raycastId.Value);
                foePosition = proximityService.GetPoint(foeProximityId);
                return true;
            }
        }
        foeProximityId = default;
        foePosition = default;
        foeRaycastState = default;
        return false;
    }

    private void FollowPath(int infantryId, InfantryAIModel behaviorModel, InfantryState infantryState, int flowFieldId) {
        var flowVector = pathfindingService.GetFlowVector(flowFieldId, infantryState.position) * infantryState.maxSpeed;
        var formationForce = formationController.GetFormationForce(behaviorModel.FormationId, infantryState.position);
        var movementVector = Vector3.ClampMagnitude(flowVector + formationForce * behaviorModel.Config.formationBlendFactor, infantryState.maxSpeed);
        infantryController.MoveTo(infantryId, infantryState.position + movementVector);
    }

}
