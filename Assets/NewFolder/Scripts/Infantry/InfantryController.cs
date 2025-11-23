using System;
using System.Collections.Generic;

using UnityEngine;

public class InfantryController {

    private readonly InfantryView view;
    private readonly CombatService combatService;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly NavigationService navigationService;
    private readonly PhysicsService physicsService;

    private int idCounter;
    private readonly Dictionary<int, InfantryModel> registry = new();

    private List<InfantryModel> diedInfantry = new();

    public InfantryController(InfantryView view, CombatService combatService, NavigationService navigationService) {
        this.view = view;
        this.combatService = combatService;
        this.navigationService = navigationService;
    }

    public IReadOnlyList<InfantryModel> DiedInfantry => diedInfantry;

    public void ClearDiedRegistry() => diedInfantry.Clear();

    public void Update() {
        ReadCombatState();
        UpdateMovements();
        ClearDeadInfantry();
        SyncPositions();
        UpdateNavigation();
        OperateInfantry();
    }

    public void SpawnInfantry(Vector3 position, bool alie, InfantryConfig config) {
        var nextId = ++idCounter;
        var model = new InfantryModel(nextId, position, config);
        registry[model.Id] = model;
        model.Health = model.MaxHealth;
        model.CombatId = combatService.RegisterAgent(model.Position, alie);
        model.AvoidanceId = localAvoidanceService.AddAgent(model.Position);
        model.PhysicsId = physicsService.RegisterPhysicsEntity(model.Position, model.PhysicsConfig.height, model.PhysicsConfig.radius);
        view.AddVisuals(model.Id, model.Position, model.VisualsPrefab);
    }

    private void DeleteInfantry(InfantryModel model) {
        registry.Remove(model.Id);
        localAvoidanceService.RemoveAgent(model.AvoidanceId);
        physicsService.UnregisterPhysicsEntity(model.PhysicsId);
        view.RemoveVisuals(model.Id);
    }

    private void ReadCombatState() {
        foreach (var model in registry.Values) {
            if (!model.IsAlive)
                continue;
                
            bool anyDamage = false;
            var combatState = combatService.GetAgentState(model.CombatId);
            
            if (combatState.exploded && model.Grounded) {
                model.Grounded = false;
                model.Health -= combatState.damage;
                physicsService.UpdatePhysicsEntityPosition(model.PhysicsId, model.Position);
                physicsService.SetPhysicsActive(model.PhysicsId, true);
                physicsService.AddExplosionForce(model.PhysicsId, 10, combatState.damageSourcePosition, 4f, 1, ForceMode.Impulse);
                anyDamage = true;
            }

            if (combatState.projectiled) {
                model.Health -= combatState.damage;
                anyDamage = true;
            }

            combatService.ClearAgentState(model.CombatId);
            if (anyDamage) {
                view.ShowTakeHit(model.Id);
            }

            if (!model.IsAlive) {
                if (combatState.projectiled) {
                    view.ShowDeathByProjectile(model.Id, combatState.damageSourcePosition, blownAway: model.Grounded);
                } else {
                    view.ShowDisolveDeath(model.Id);
                }
                
                combatService.UnregisterAgent(model.CombatId);
                diedInfantry.Add(model);
            }
        }
    }

    private void UpdateMovements() {
        foreach (var model in registry.Values) {
            var physicsPose = physicsService.GetEntityPose(model.PhysicsId);
            var keepFlying = !model.Grounded && physicsPose.InMotion;
            var becomeGrounded = !model.Grounded && !physicsPose.InMotion;
            var keepsGrouned = model.Grounded && !physicsPose.InMotion;
            
            if (keepFlying) {
                model.Position = physicsPose.Position;
                model.Rotation = physicsPose.Rotation;
            } else if (becomeGrounded) {
                model.Grounded = true;
                model.Position = physicsService.GetGroundPosition(model.Position);
                model.Rotation = Quaternion.identity;
                physicsService.SetPhysicsActive(model.PhysicsId, false);
            } else if (keepsGrouned && model.IsAlive) {
                localAvoidanceService.GetAgentPositionAndRotation(model.AvoidanceId, out var pos, out var rot);
                model.Position = pos;
                model.Rotation = rot;
            }
        }
    }

    private void ClearDeadInfantry() {
        List<InfantryModel> infantryToRemove = new();
        foreach (var model in registry.Values) {
            if (!model.IsAlive && model.Grounded) {
                infantryToRemove.Add(model);
            }
        }
        foreach (var model in infantryToRemove) {
            DeleteInfantry(model);
        }
    }

    private void SyncPositions() {
        foreach (var model in registry.Values) {
            localAvoidanceService.SetAgentPosition(model.AvoidanceId, model.Position);
            view.UpdateTransform(model.Id, model.Position, model.Rotation);
            if (model.IsAlive) {
                combatService.UpdateAgentPosition(model.CombatId, model.Position);
            }
        }
    }

    private void UpdateNavigation() {
        foreach (var model in registry.Values) {    
            var goalNavigationVector = navigationService.GetFlowVector(model.Position);
            localAvoidanceService.SetPreferedVelocity(model.AvoidanceId, goalNavigationVector);
        }
    }

    private void OperateInfantry() {
        foreach (var model in registry.Values) {    
            if (!model.Grounded || !model.IsAlive)
                continue;
            
            if (!combatService.GetClosestEnemyAgentInRange(model.CombatId, 2, out var closestFoe))
                continue;
                
            if (model.LastAttackTime + model.AttackCooldown < Time.time) {
                combatService.ApplyDirectDamage(model.CombatId, closestFoe.id, model.Damage);
                view.ShowDirectFrontAttack(model.Id);
            }
        }
    }

}
