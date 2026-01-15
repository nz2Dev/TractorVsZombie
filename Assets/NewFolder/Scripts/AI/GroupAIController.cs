using System;
using System.Collections.Generic;

using UnityEngine;

public class GroupAIController {
    
    private readonly InfantryController infantryController;
    private readonly NavigationSystem navigationSystem;
    private readonly CombatService combatService;

    private int groupIdCounter;
    private Dictionary<int, GroupAIModel> registry = new();

    public GroupAIController(InfantryController infantryController, NavigationSystem navigationSystem, CombatService combatService) {
        this.infantryController = infantryController;
        this.navigationSystem = navigationSystem;
        this.combatService = combatService;
    }

    public void Update() {
        ValidateControledInfantry();
        OperateInfantry();
    }

    public int AddGroup() {
        var nextGroupId = ++groupIdCounter;
        var groupModel = new GroupAIModel();
        groupModel.NavigationFormationId = navigationSystem.CreateFormation();
        registry[nextGroupId] = groupModel;
        return nextGroupId;
    }

    public void AddInfantryToGroup(int groupId, int infantryId) {
        var groupModel = registry[groupId];
        var state = infantryController.GetInfantryState(infantryId);
        var config = infantryController.GetAvoidanceConfig(infantryId);
        var navigationAgentId = navigationSystem.AddAgent(state.position, config.maxSpeed, config);
        navigationSystem.AssignAgentToFormation(navigationAgentId, groupModel.NavigationFormationId);
        groupModel.ControlledInfantries.Add(new ControledInfantry {
            infantryId = infantryId,
            navigationAgentId = navigationAgentId
        });
    }

    public int GetGroupSize(int groupId) {
        return registry[groupId].ControlledInfantries.Count;
    }

    private void ValidateControledInfantry() {
        var toRemoveBuffer = new List<ControledInfantry>(12);
        foreach (var aiGroup in registry.Values) {
            toRemoveBuffer.Clear();
            foreach (var controledInfantry in aiGroup.ControlledInfantries) {
                if (!infantryController.IsExist(controledInfantry.infantryId)) {
                    toRemoveBuffer.Add(controledInfantry);
                }
            }
            foreach (var invalidControledInfantry in toRemoveBuffer) {
                aiGroup.ControlledInfantries.Remove(invalidControledInfantry);
                navigationSystem.RemoveAgent(invalidControledInfantry.navigationAgentId);
            }
        }
    }

    private void OperateInfantry() {
        foreach (var aiGroup in registry.Values) {
            foreach (var controlledInfantry in aiGroup.ControlledInfantries) {
                var infantryState = infantryController.GetInfantryState(controlledInfantry.infantryId);
                if (!infantryState.isGrounded || !infantryState.isAlive)
                    continue;
                
                var navigationVelocity = navigationSystem.GetComputedVelocity(controlledInfantry.navigationAgentId);
                infantryController.Move(controlledInfantry.infantryId, navigationVelocity);
                navigationSystem.SetNextPosition(controlledInfantry.navigationAgentId, infantryState.position);

                if (combatService.GetClosestEnemyAgentInRange(infantryState.combatId, 2, out var closestFoe)) {
                    infantryController.Attack(controlledInfantry.infantryId, closestFoe.id);
                }
            }
        }
    }

}