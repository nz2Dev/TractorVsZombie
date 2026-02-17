using System;
using System.Collections.Generic;

using UnityEngine;

public class CommanderSystem {

    private readonly CombatSystem combatSystem;
    private readonly InfantryController infantryController;
    private readonly PathfindingService pathfindingService;

    private int idCounter;
    private readonly Dictionary<int, CommanderState> registry = new();

    public CommanderSystem(InfantryController infantryController, PathfindingService pathfindingService, CombatSystem combatSystem) {
        this.infantryController = infantryController;
        this.pathfindingService = pathfindingService;
        this.combatSystem = combatSystem;
    }

    public void Update() {
        ValidateSubordinates();
        ComputeFormations();
        ProcessCommands();
    }

    public int CreateCommander() {
        var nextGroupId = ++idCounter;
        var state = new CommanderState();
        state.FlowFieldId = pathfindingService.CreateFlowField(Vector3.zero);
        state.ChaseCenter = true;
        registry[nextGroupId] = state;
        return nextGroupId;
    }

    public void AddSubordinate(int commanderId, int infantryId) {
        var commanderState = registry[commanderId];
        commanderState.SubordinateIds.Add(infantryId);
    }

    public void SetStrategy(int commanderId, bool chaseCenter, Vector3 position) {
        var commanderState = registry[commanderId];
        commanderState.ChaseCenter = chaseCenter;
        pathfindingService.UpdateGoal(commanderState.FlowFieldId, position);
    }

    public CommanderSnapshot GetCommanderSnapshot(int commanderId) {
        var commanderState = registry[commanderId];
        return new CommanderSnapshot {
            subordinateCount = commanderState.SubordinateIds.Count,
            isChasingCenter = commanderState.ChaseCenter,
        };
    }

    private void ValidateSubordinates() {
        foreach (var commanderState in registry.Values) {
            for (int i = commanderState.SubordinateIds.Count - 1; i >= 0; i--) {
                var infantryId = commanderState.SubordinateIds[i];
                if (!infantryController.IsExist(infantryId)) {
                    commanderState.SubordinateIds.RemoveAt(i);
                }
            }
        }
    }

    private void ComputeFormations() {
        foreach (var commanderState in registry.Values) {
            var count = 0;
            var sumPosition = Vector3.zero;
            var sumDirection = Vector3.zero;

            foreach (var infantryId in commanderState.SubordinateIds) {
                var infantryState = infantryController.GetInfantryState(infantryId);
                sumPosition += infantryState.position;
                sumDirection += infantryState.movementVelocity;
                count++;
            }

            commanderState.FormationCohesionInput = new CohesionInput {
                center = sumPosition / count,
                direction = (sumDirection / count).normalized,
            };
        }
    }

    private void ProcessCommands() {
        foreach (var commanderState in registry.Values) {
            foreach (var infantryId in commanderState.SubordinateIds) {
                var infantry = infantryController.GetInfantryState(infantryId);
                if (!infantry.isAlive || !infantry.isGrounded)
                    continue;

                var flowVector = pathfindingService.GetFlowVector(commanderState.FlowFieldId, infantry.position);
                var movementIntent = Steering.CohesionSteering(infantry.position, flowVector, infantry.maxSpeed, commanderState.FormationCohesionInput);
                infantryController.Move(infantryId, movementIntent);

                if (combatSystem.GetClosestEnemyAgentInRange(infantry.combatId, 2, out var closestFoe)) {
                    infantryController.Attack(infantryId, closestFoe.id);
                }
            }
        }
    }

}