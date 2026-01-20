using System.Collections.Generic;
using UnityEngine;

public class BehaviorSystem {

    private readonly NavigationSystem navigationSystem;
    private readonly InfantryController infantryController;
    private readonly CombatService combatService;

    private int nextId;
    private readonly Dictionary<int, BehaviorActor> registry = new();

    public BehaviorSystem(NavigationSystem navigationSystem, InfantryController infantryController, CombatService combatService) {
        this.navigationSystem = navigationSystem;
        this.infantryController = infantryController;
        this.combatService = combatService;
    }

    public void Update() {
        ProcessBehaviors();
    }

    public int CreateActor(int infantryId, int flowFieldId) {
        var state = infantryController.GetInfantryState(infantryId);
        var config = infantryController.GetAvoidanceConfig(infantryId);
        var navigationAgentId = navigationSystem.AddAgent(state.position, flowFieldId, config.maxSpeed, config);
        var id = ++nextId;
        registry[id] = new BehaviorActor(id, infantryId, navigationAgentId);
        return id;
    }

    public void SetSteeringInput(int actorId, SteeringInput input) {
        registry[actorId].SteeringInput = input;
    }

    public void RemoveActor(int id) {
        var agent = registry[id];
        navigationSystem.RemoveAgent(agent.NavigationAgentId);
        registry.Remove(id);
    }

    public void ProcessBehaviors() {
        foreach (var actor in registry.Values) {
            // *currently it's implicit chase behavior execution for every actor* //
            var infantryState = infantryController.GetInfantryState(actor.InfantryId);
            if (!infantryState.isAlive || !infantryState.isGrounded) 
                continue;

            navigationSystem.SetNextSteering(actor.NavigationAgentId, actor.SteeringInput);
            navigationSystem.SetNextPosition(actor.NavigationAgentId, infantryState.position);

            var navigationVelocity = navigationSystem.GetComputedVelocity(actor.NavigationAgentId);
            infantryController.Move(actor.InfantryId, navigationVelocity);

            if (combatService.GetClosestEnemyAgentInRange(infantryState.combatId, 2, out var closestFoe)) {
                infantryController.Attack(actor.InfantryId, closestFoe.id);
            }
        }
    }
}
