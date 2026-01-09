using System;
using System.Collections.Generic;

using UnityEngine;

public class InfantryController {

    private readonly InfantryView view;
    private readonly CombatService combatService;
    private readonly BodySimulator bodyController;

    private int idCounter;
    private readonly Dictionary<int, InfantryModel> registry = new();

    private List<InfantryModel> diedInfantry = new();

    public InfantryController(CombatService combatService, BodySimulator bodyController, InfantryView view) {
        this.combatService = combatService;
        this.bodyController = bodyController;
        this.view = view;
    }

    public IReadOnlyList<InfantryModel> DiedInfantry => diedInfantry;
    public int InfantryCount => registry.Count;

    public void ClearDiedRegistry() => diedInfantry.Clear();

    public void WriteDeadInfantryFiltered(List<int> referencedList) {
        referencedList.RemoveAll(id => !registry.ContainsKey(id));
    }

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
        model.Health = model.MaxHealth;
        model.CombatId = combatService.RegisterAgent(position, alie);
        model.BodyId = bodyController.SpawnBody(position, model.BodyConfig);
        view.AddVisuals(model.Id, position, model.VisualsPrefab);
        return model.Id;
    }

    public void Move(int infantryId, Vector3 direction) {
        var model = registry[infantryId];
        bodyController.SetPreferedVelocity(model.BodyId, direction);
    }

    public void Attack(int infantryId, int targetCombatId) {
        var model = registry[infantryId];
        if (model.LastAttackTime + model.AttackCooldown < Time.time) {
            model.LastAttackTime = Time.time;
            combatService.ApplyDirectDamage(model.CombatId, targetCombatId, model.Damage);
            view.ShowDirectFrontAttack(model.Id);
        }
    }

    public InfantryState GetInfantryState(int infantryId) {
        var model = registry[infantryId];
        return new InfantryState {
            position = model.BodyState.position,
            isAlive = model.IsAlive,
            isGrounded = model.BodyState.grounded,
            combatId = model.CombatId,
            bodyId = model.BodyId,
        };
    }

    private void ClearDeadInfantry() {
        List<InfantryModel> infantryToRemove = new();
        
        foreach (var model in registry.Values) 
            if (!model.IsAlive && model.BodyState.grounded) 
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
            if (!model.IsAlive)
                continue;
                
            bool anyDamage = false;
            var combatState = combatService.GetAgentState(model.CombatId);
            
            if (combatState.exploded && model.BodyState.grounded) {
                model.Health -= combatState.damage;
                bodyController.ApplyImpulse(model.BodyId, combatState.damageSourcePosition);
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
                bodyController.DisableRecovery(model.BodyId);
                if (combatState.projectiled) {
                    view.ShowDeathByProjectile(model.Id, combatState.damageSourcePosition, blownAway: model.BodyState.grounded);
                } else {
                    view.ShowDisolveDeath(model.Id);
                }
                combatService.UnregisterAgent(model.CombatId);
                diedInfantry.Add(model);
            }
        }
    }

    private void SyncPositions() {
        foreach (var model in registry.Values) {
            view.UpdateTransform(model.Id, model.BodyState.position, model.BodyState.rotation);
            if (model.IsAlive) {
                combatService.UpdateAgentPosition(model.CombatId, model.BodyState.position);
            }
        }
    }

}
