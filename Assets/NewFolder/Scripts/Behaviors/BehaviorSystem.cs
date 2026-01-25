using System.Collections.Generic;
using UnityEngine;

public class BehaviorSystem {

    private readonly NavigationSystem navigationSystem;
    private readonly InfantryController infantryController;
    private readonly CombatService combatService;

    private int nextId;
    private readonly Dictionary<int, BehaviorActor> registry = new();
    private readonly List<BehaviorActor> removalBuffer = new(64);

    public BehaviorSystem(NavigationSystem navigationSystem, InfantryController infantryController, CombatService combatService) {
        this.navigationSystem = navigationSystem;
        this.infantryController = infantryController;
        this.combatService = combatService;
    }
    

    public void Update() {
        ValidateActors();
        ProcessBehaviors();
    }

    public bool IsActorExist(int id) {
        return registry.ContainsKey(id);
    }

    public int CreateActor(int infantryId) {
        var state = infantryController.GetInfantryState(infantryId);
        var config = infantryController.GetAvoidanceConfig(infantryId);
        var navigationAgentId = navigationSystem.AddAgent(state.position, config.maxSpeed, config);
        var id = ++nextId;
        registry[id] = new BehaviorActor(id, infantryId, navigationAgentId); 
        // TODO: use consistent state initialization
        // model.field = value
        return id;
    }

    public void ChaseInFormation(List<int> actorIds, MarkerId markerId) {
        var count = 0;
        var sumPosition = Vector3.zero;
        var sumDirection = Vector3.zero;
        
        foreach (var actorId in actorIds) {
            var actor = registry[actorId];
            var infantryState = infantryController.GetInfantryState(actor.InfantryId);
            sumPosition += infantryState.position;
            sumDirection += infantryState.movementVelocity;
            count++;
        }

        var formationSteering = new SteeringInput {
            CohesionCenter = sumPosition / count,
            AlignmentDirection = (sumDirection / count).normalized,
        };

        foreach (var actorId in actorIds) {
            var actor = registry[actorId];
            actor.SteeringInput = formationSteering;
            actor.TargetMarkerId = markerId;
        }
    }

    private void ValidateActors() {
        removalBuffer.Clear();
        foreach (var actor in registry.Values)
            if (!infantryController.IsExist(actor.InfantryId))
                removalBuffer.Add(actor);
        
        foreach (var actor in removalBuffer) {
            navigationSystem.RemoveAgent(actor.NavigationAgentId);
            registry.Remove(actor.Id);
        }
    }

    private void ProcessBehaviors() {
        foreach (var actor in registry.Values) {
            // *currently it's implicit chase behavior execution for every actor* //
            var infantryState = infantryController.GetInfantryState(actor.InfantryId);
            if (!infantryState.isAlive || !infantryState.isGrounded) 
                continue;

            navigationSystem.SetDestination(actor.NavigationAgentId, actor.TargetMarkerId);
            navigationSystem.SetNextPosition(actor.NavigationAgentId, infantryState.position);
            navigationSystem.SetNextSteering(actor.NavigationAgentId, actor.SteeringInput);

            var navigationVelocity = navigationSystem.GetComputedVelocity(actor.NavigationAgentId);
            infantryController.Move(actor.InfantryId, navigationVelocity);

            if (combatService.GetClosestEnemyAgentInRange(infantryState.combatId, 2, out var closestFoe)) {
                infantryController.Attack(actor.InfantryId, closestFoe.id);
            }
        }
    }

}
