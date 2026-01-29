using System;
using System.Collections.Generic;

using UnityEngine;

public class InfantryController {

    private readonly InfantryView view;
    private readonly CombatSystem combatSystem;
    private readonly BodySimulator bodyController;

    private int idCounter;
    private readonly Dictionary<int, InfantryModel> registry = new();

    private List<InfantryModel> diedInfantry = new();

    public InfantryController(CombatSystem combatSystem, BodySimulator bodyController, InfantryView view) {
        this.combatSystem = combatSystem;
        this.bodyController = bodyController;
        this.view = view;
    }

    public IReadOnlyList<InfantryModel> DiedInfantry => diedInfantry;
    public int InfantryCount => registry.Count;

    public void ClearDiedRegistry() => diedInfantry.Clear();
    public bool IsExist(int infantryId) => registry.ContainsKey(infantryId);

    public void Update() {
        ReadBodyState();
        ClearDeadInfantry();
        ReadCombatState();
        SyncPositions();
    }

    public int SpawnInfantry(Vector3 position, bool alie, InfantryConfig config) {
        var nextId = ++idCounter;
        var model = new InfantryModel(nextId, config);
        registry[model.Id] = model;
        model.CombatId = combatSystem.RegisterAgent(position, alie, model.MaxHealthConfig);
        model.BodyId = bodyController.SpawnBody(position, model.BodyConfig);
        view.AddVisuals(model.Id, position, model.VisualsPrefab);
        return model.Id;
    }

    public void Move(int infantryId, Vector3 velocity) {
        var model = registry[infantryId];
        bodyController.ApplyMovement(model.BodyId, velocity);
    }

    public void Attack(int infantryId, int targetCombatId) {
        var model = registry[infantryId];
        if (model.LastAttackTime + model.AttackCooldown < Time.time) {
            model.LastAttackTime = Time.time;
            combatSystem.ApplyDirectDamage(model.CombatId, targetCombatId, model.Damage);
            view.ShowDirectFrontAttack(model.Id);
        }
    }

    public InfantryState GetInfantryState(int infantryId) {
        var model = registry[infantryId];
        return new InfantryState {
            position = model.BodyState.position,
            movementVelocity = model.BodyState.movementVelocity,
            isAlive = !model.IsDead,
            isGrounded = model.BodyState.grounded,
            combatId = model.CombatId,
            bodyId = model.BodyId,
        };
    }

    public AgentAvoidanceConfig GetAvoidanceConfig(int infantryId) {
        return registry[infantryId].AgentAvoidanceConfig;
    }

    private void ClearDeadInfantry() {
        List<InfantryModel> infantryToRemove = new();
        
        foreach (var model in registry.Values) 
            if (model.IsDead && model.BodyState.grounded) 
                infantryToRemove.Add(model);
            
        foreach (var model in infantryToRemove)
            DeleteInfantry(model);
    }

    private void DeleteInfantry(InfantryModel model) {
        registry.Remove(model.Id);
        bodyController.DeleteBody(model.BodyId);
        view.RemoveVisuals(model.Id);
    }

    private void ReadBodyState() {
        foreach (var model in registry.Values) {
            model.BodyState = bodyController.ReadBodyState(model.BodyId);
        }
    }

    private void ReadCombatState() {
        foreach (var model in registry.Values) {
            if (model.IsDead)
                continue;
            
            var combatOutput = combatSystem.GetCombatOutput(model.CombatId);
            if (combatOutput.wasExploded && model.BodyState.grounded) {
                bodyController.ApplyImpulse(model.BodyId, combatOutput.damageSourcePosition);
            }

            // base visual effects on combat effects irregarding of logic damage
            if (combatOutput.wasProjectiled || combatOutput.wasExploded) {
                view.ShowTakeHit(model.Id);
            }

            if (combatOutput.damageWasFatal) {
                model.IsDead = true;
                bodyController.DisableRecovery(model.BodyId);
                
                if (combatOutput.wasProjectiled) {
                    view.ShowDeathByProjectile(model.Id, combatOutput.damageSourcePosition, blownAway: model.BodyState.grounded);
                } else {
                    view.ShowDisolveDeath(model.Id);
                }

                combatSystem.UnregisterAgent(model.CombatId); // TODO: keep registered, add combat system queries filters for IsAlive
                diedInfantry.Add(model);
            }
        }
    }

    private void SyncPositions() {
        foreach (var model in registry.Values) {
            view.UpdateTransform(model.Id, model.BodyState.position, model.BodyState.rotation);
            if (!model.IsDead) {
                combatSystem.UpdateAgentPosition(model.CombatId, model.BodyState.position);
            }
        }
    }

}
