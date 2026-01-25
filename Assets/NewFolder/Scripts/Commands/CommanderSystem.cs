using System;
using System.Collections.Generic;

using UnityEngine;

public class CommanderSystem {
    
    private readonly BehaviorSystem behaviorSystem;
    private readonly NavigationSystem navigationSystem;

    private int idCounter;
    private readonly Dictionary<int, CommanderState> registry = new();

    public CommanderSystem(BehaviorSystem behaviorSystem, NavigationSystem navigationSystem) {
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

    public void AddSubordinate(int commanderId, int actorId) {
        var commanderState = registry[commanderId];
        commanderState.Subordinates.Add(new Subordinate { behaviorActorId = actorId });
        commanderState.SubordinateActorIds.Add(actorId);
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
                if (!behaviorSystem.IsActorExist(subordinate.behaviorActorId)) {
                    commanderState.Subordinates.RemoveAt(i);
                    commanderState.SubordinateActorIds.RemoveAt(i);
                }
            }
        }
    }

    private void ProcessCommanders() {
        foreach (var commanderState in registry.Values) {
            behaviorSystem.ChaseInFormation(commanderState.SubordinateActorIds, commanderState.CommonTargetMarkerId);
        }
    }

}