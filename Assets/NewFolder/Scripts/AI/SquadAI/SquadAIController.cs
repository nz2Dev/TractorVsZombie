using System;
using System.Collections.Generic;

using UnityEngine;

public class SquadAIController {

    private readonly CombatSystem combatSystem;
    private readonly InfantryController infantryController;
    private readonly PathfindingService pathfindingService;

    private int idCounter;
    private readonly Dictionary<int, SquadAIState> registry = new();

    public SquadAIController(InfantryController infantryController, PathfindingService pathfindingService, CombatSystem combatSystem) {
        this.infantryController = infantryController;
        this.pathfindingService = pathfindingService;
        this.combatSystem = combatSystem;
    }

    public void Update() {
        ValidateSubordinates();
        ProcessCommands();
    }

    public int CreateSquad() {
        var nextId = ++idCounter;
        var state = new SquadAIState();
        state.FlowFieldId = pathfindingService.CreateFlowField(Vector3.zero);
        state.Formation = new CohesionFormation(64);
        state.ChaseCenter = true;
        registry[nextId] = state;
        return nextId;
    }

    public void AddSubordinate(int id, int infantryId) {
        var state = registry[id];
        state.SubordinateIds.Add(infantryId);
    }

    public void SetStrategy(int id, bool chaseCenter, Vector3 position) {
        var state = registry[id];
        state.ChaseCenter = chaseCenter;
        pathfindingService.UpdateGoal(state.FlowFieldId, position);
    }

    public SquadAISnapshot GetSquadSnapshot(int id) {
        var state = registry[id];
        return new SquadAISnapshot {
            subordinateCount = state.SubordinateIds.Count,
            isChasingCenter = state.ChaseCenter,
        };
    }

    private void ValidateSubordinates() {
        foreach (var state in registry.Values) {
            for (int i = state.SubordinateIds.Count - 1; i >= 0; i--) {
                var infantryId = state.SubordinateIds[i];
                if (!infantryController.IsExist(infantryId)) {
                    state.SubordinateIds.RemoveAt(i);
                }
            }
        }
    }

    private void ProcessCommands() {
        foreach (var state in registry.Values) {
            state.Formation.Clear();
            foreach (var infantryId in state.SubordinateIds) {
                var infantryState = infantryController.GetInfantryState(infantryId);
                state.Formation.AddMember(infantryState.position, infantryState.movementVelocity, infantryState.maxSpeed);
            }
            state.Formation.Compute();

            for (int subordinateIndex = 0; subordinateIndex < state.SubordinateIds.Count; subordinateIndex++) {
                var infantryId = state.SubordinateIds[subordinateIndex];
                var infantry = infantryController.GetInfantryState(infantryId);
                if (!infantry.isAlive || !infantry.isGrounded)
                    continue;

                var flowVector = pathfindingService.GetFlowVector(state.FlowFieldId, infantry.position);
                var formationVector = state.Formation.GetFormationVector(subordinateIndex);
                infantryController.Move(infantryId, Vector3.Lerp(flowVector, formationVector, 0.3f));

                if (combatSystem.GetClosestEnemyAgentInRange(infantry.combatId, 2, out var closestFoe)) {
                    infantryController.Attack(infantryId, closestFoe.id);
                }
            }
        }
    }

}