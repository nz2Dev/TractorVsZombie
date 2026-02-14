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

    public void AddSubordinate(int commanderId, int infantryId) {
        var commanderState = registry[commanderId];
        var infantryActorId = behaviorSystem.CreateActor(infantryId);
        commanderState.SubordinateActorIds.Add(infantryActorId);
    }

    public void SetStrategy(int commanderId, bool chaseCenter, Vector3 position) {
        var commanderState = registry[commanderId];
        commanderState.ChaseCenter = chaseCenter;
        navigationSystem.UpdateMarkerPosition(commanderState.CommonTargetMarkerId, position);
    }

    public CommanderSnapshot GetCommanderSnapshot(int commanderId) {
        var commanderState = registry[commanderId];
        return new CommanderSnapshot {
            subordinateCount = commanderState.SubordinateActorIds.Count,
            isChasingCenter = commanderState.ChaseCenter,
        };
    }

    private void ValidateSubordinates() {
        foreach (var commanderState in registry.Values) {
            for (int i = commanderState.SubordinateActorIds.Count - 1; i >= 0; i--) {
                var actorId = commanderState.SubordinateActorIds[i];
                if (!behaviorSystem.IsActorExist(actorId)) {
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