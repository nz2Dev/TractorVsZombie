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
        ComputeFormations();
        ProcessCommands();
    }

    public int CreateSquad() {
        var nextId = ++idCounter;
        var state = new SquadAIState();
        state.FlowFieldId = pathfindingService.CreateFlowField(Vector3.zero);
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

    private void ComputeFormations() {
        foreach (var state in registry.Values) {
            var count = 0;
            var sumPosition = Vector3.zero;
            var sumDirection = Vector3.zero;

            foreach (var infantryId in state.SubordinateIds) {
                var infantryState = infantryController.GetInfantryState(infantryId);
                sumPosition += infantryState.position;
                sumDirection += infantryState.movementVelocity;
                count++;
            }

            state.FormationCohesionInput = new CohesionInput {
                center = sumPosition / count,
                direction = (sumDirection / count).normalized,
            };
        }
    }

    private void ProcessCommands() {
        foreach (var state in registry.Values) {
            foreach (var infantryId in state.SubordinateIds) {
                var infantry = infantryController.GetInfantryState(infantryId);
                if (!infantry.isAlive || !infantry.isGrounded)
                    continue;

                var flowVector = pathfindingService.GetFlowVector(state.FlowFieldId, infantry.position);
                var movementIntent = Steering.CohesionSteering(infantry.position, flowVector, infantry.maxSpeed, state.FormationCohesionInput);
                infantryController.Move(infantryId, movementIntent);

                if (combatSystem.GetClosestEnemyAgentInRange(infantry.combatId, 2, out var closestFoe)) {
                    infantryController.Attack(infantryId, closestFoe.id);
                }
            }
        }
    }

}