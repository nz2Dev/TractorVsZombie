using System;
using System.Collections.Generic;

using UnityEngine;

public class CommanderSystem {
    
    private readonly BehaviorSystem behaviorSystem;
    private readonly InfantryController infantryController;
    private readonly NavigationSystem navigationSystem;

    private int idCounter;
    private readonly Dictionary<int, CommanderState> registry = new();

    public CommanderSystem(InfantryController infantryController, BehaviorSystem behaviorSystem, NavigationSystem navigationSystem) {
        this.infantryController = infantryController;
        this.behaviorSystem = behaviorSystem;
        this.navigationSystem = navigationSystem;
    }

    public void Update() {
        ValidateSubordinates();
        ProcessCommanders();
    }

    public int CreateCommander() {
        var nextGroupId = ++idCounter;
        var state = new CommanderState();
        state.CommonTargetMarkerId = navigationSystem.CreateMarker(Vector3.zero);
        registry[nextGroupId] = state;
        return nextGroupId;
    }

    public void AddSubordinate(int commanderId, int infantryId) {
        var commanderState = registry[commanderId];
        var actorId = behaviorSystem.CreateActor(infantryId);
        commanderState.Subordinates.Add(new Subordinate {
            infantryId = infantryId,
            behaviorActorId = actorId
        });
    }

    public void SetStrategy(int commanderId, bool chaseCenter, Vector3 position) {
        var commanderState = registry[commanderId];
        commanderState.ChaseCenter = chaseCenter;
        navigationSystem.UpdateMarkerPosition(commanderState.CommonTargetMarkerId, position);
    }

    public CommanderSnapshot GetCommanderSnapshot(int commanderId) {
        var commanderState = registry[commanderId];
        return new CommanderSnapshot {
            subordinateCount = commanderState.Subordinates.Count,
            isChasingCenter = commanderState.ChaseCenter,
        };
    }

    private void ValidateSubordinates() {
        foreach (var commanderState in registry.Values) {
            for (int i = commanderState.Subordinates.Count - 1; i >= 0; i--) {
                var subordinate = commanderState.Subordinates[i];
                if (!infantryController.IsExist(subordinate.infantryId)) {
                    behaviorSystem.RemoveActor(subordinate.behaviorActorId);
                    commanderState.Subordinates.RemoveAt(i);
                }
            }
        }
    }

    private void ProcessCommanders() {
        foreach (var commanderState in registry.Values) {
            commanderState.FormationSteering = ComputeFormationSteering(commanderState);
            foreach (var subordinate in commanderState.Subordinates) {
                behaviorSystem.SetNavigationInput(subordinate.behaviorActorId, 
                    commanderState.FormationSteering,
                    commanderState.CommonTargetMarkerId);
            }
        }
    }

    // Idea: this compute is done in behavior system, and returns some sort of "Context", that later can be assigned to actor, or via specific behavior
    // This way, navigation and movement will be transparent to commander.
    // Idea/2: Or API to form a formation is directly handled in behavior system, commander only decides who belong in formations and when.
    private SteeringInput ComputeFormationSteering(CommanderState state) {
        int count = 0;
        Vector3 sumPosition = Vector3.zero;
        Vector3 sumDirection = Vector3.zero;

        foreach (var subordinate in state.Subordinates) {
            var infantryState = infantryController.GetInfantryState(subordinate.infantryId);
            sumPosition += infantryState.position;
            sumDirection += infantryState.movementVelocity;
            count++;
        }

        return new SteeringInput {
            CohesionCenter = sumPosition / count,
            AlignmentDirection = (sumDirection / count).normalized,
        };
    }


}