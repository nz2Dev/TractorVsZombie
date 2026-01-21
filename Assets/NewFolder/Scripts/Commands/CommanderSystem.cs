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
        UpdateGoals();
        ProcessCommanders();
    }

    public int CreateCommander(Vector3 origin) {
        var nextGroupId = ++idCounter;
        var state = new CommanderState();
        state.Origin = origin;
        state.CommonDestinationId = navigationSystem.CreateDestination(Vector3.zero);
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

    public int GetSubordinates(int commanderId) {
        return registry[commanderId].Subordinates.Count;
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

    private void UpdateGoals() {
        if (Input.GetKeyDown(KeyCode.R)) {
            foreach (var commanderState in registry.Values) {
                commanderState.ChaseCenter = !commanderState.ChaseCenter;
                var goalPosition = commanderState.ChaseCenter ? Vector3.zero : commanderState.Origin;
                navigationSystem.UpdateDestinationPosition(commanderState.CommonDestinationId, goalPosition);
            }
        }
    }

    private void ProcessCommanders() {
        foreach (var commanderState in registry.Values) {
            commanderState.FormationSteering = ComputeFormationSteering(commanderState);
            foreach (var subordinate in commanderState.Subordinates) {
                behaviorSystem.SetNavigationInput(subordinate.behaviorActorId, 
                    commanderState.FormationSteering,
                    commanderState.CommonDestinationId);
            }
        }
    }

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