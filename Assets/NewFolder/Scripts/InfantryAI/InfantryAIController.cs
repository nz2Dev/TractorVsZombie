using System;
using System.Collections.Generic;

using UnityEngine;

public class InfantryAIController {
    
    private readonly InfantryController infantryController;
    private readonly NavigationSystem navigationSystem;
    private readonly CombatService combatService;

    private int lastFormationCount;
    private int navigationFormationId;
    private readonly List<int> controlledInfantryIds = new ();
    private readonly Dictionary<int, int> infantryToNavAgent = new();

    public InfantryAIController(InfantryController infantryController, NavigationSystem navigationSystem, CombatService combatService) {
        this.infantryController = infantryController;
        this.navigationSystem = navigationSystem;
        this.combatService = combatService;
    }

    public void Update() {
        ValidateInfantryIds();
        UpdateFormation();
        OperateInfantry();
    }

    public void InitFormations() {
        lastFormationCount = 0;
        navigationFormationId = navigationSystem.CreateFormation();
    }

    public void SetGoal(Vector3 position) {
        navigationSystem.SetGoal(position);
    }

    public void TakeUnderControl(int infantryId) {
        var state = infantryController.GetInfantryState(infantryId);
        var config = infantryController.GetAvoidanceConfig(infantryId);
        var navId = navigationSystem.AddAgent(state.position, config.maxSpeed, config);
        navigationSystem.AssignAgentToFormation(navId, navigationFormationId);
        lastFormationCount++;
        infantryToNavAgent[infantryId] = navId;
        controlledInfantryIds.Add(infantryId);
    }

    private void UpdateFormation() {
        if (lastFormationCount > 30) {
            lastFormationCount = 0;
            navigationFormationId = navigationSystem.CreateFormation();
        }
    }

    private void ValidateInfantryIds() {
        infantryController.WriteDeadInfantryFiltered(controlledInfantryIds);
        
        List<int> toRemove = new();
        foreach(var id in infantryToNavAgent.Keys) {
            if (!controlledInfantryIds.Contains(id)) {
                toRemove.Add(id);
            }
        }
        
        foreach(var id in toRemove) {
            navigationSystem.RemoveAgent(infantryToNavAgent[id]);
            infantryToNavAgent.Remove(id);
        }
    }

    private void OperateInfantry() {
        for (int i = 0; i < controlledInfantryIds.Count; i++) {
            var infantryId = controlledInfantryIds[i];
            var state = infantryController.GetInfantryState(infantryId);
            if (!state.isGrounded || !state.isAlive)
                continue;
            
            if (infantryToNavAgent.TryGetValue(infantryId, out var navId)) {
                var navigationVelocity = navigationSystem.GetComputedVelocity(navId);
                infantryController.Move(infantryId, navigationVelocity);
                navigationSystem.SetNextPosition(navId, state.position);
            }

            if (combatService.GetClosestEnemyAgentInRange(state.combatId, 2, out var closestFoe)) {
                infantryController.Attack(infantryId, closestFoe.id);
            }
        }
    }

}