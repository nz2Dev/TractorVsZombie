using System;
using System.Collections.Generic;

using UnityEngine;

public class InfantryController {

    private readonly InfantryView view;
    private readonly CombatSystem combatSystem;
    private readonly LocalAvoidanceService avoidanceService;
    private readonly PhysicsService physicsService;
    private readonly RewardController rewardController;

    private int idCounter;
    private readonly Dictionary<int, InfantryModel> registry = new();

    public InfantryController(CombatSystem combatSystem, InfantryView view, RewardController rewardController, PhysicsService physicsService, LocalAvoidanceService avoidanceService) {
        this.combatSystem = combatSystem;
        this.view = view;
        this.rewardController = rewardController;
        this.physicsService = physicsService;
        this.avoidanceService = avoidanceService;
    }

    public int InfantryCount => registry.Count;
    public bool IsExist(int infantryId) => registry.ContainsKey(infantryId);

    public void Update() {
        UpdateMovements();
        ClearDeadInfantry();
        ReadCombatState();
        SyncPositions();
    }

    public int SpawnInfantry(Vector3 position, InfantryConfig config) {
        var nextId = ++idCounter;
        var model = new InfantryModel(nextId, config);
        registry[model.Id] = model;
        model.Position = position;
        model.CombatId = combatSystem.RegisterAgent(position, config.alie, model.Config.maxHealth);
        model.BodyPhysicsId = physicsService.RegisterPhysicsEntity(position, model.Config.bodyData);
        model.AvoidanceId = avoidanceService.AddAgent(position, config.agentAvoidanceConfig);
        view.AddVisuals(model.Id, position, model.Config.visualsPrefab);
        return model.Id;
    }

    public void Move(int infantryId, Vector3 velocity) {
        var model = registry[infantryId];
        avoidanceService.SetPreferedVelocity(model.AvoidanceId, velocity);
    }

    public void Position(int infantryId, Vector3 position) {
        var model = registry[infantryId];
        model.Position = position;
        physicsService.UpdatePhysicsEntityPosition(model.BodyPhysicsId, position);
    }

    public void Attack(int infantryId, int targetCombatId) {
        var model = registry[infantryId];
        if (model.LastAttackTime + model.Config.attackCooldown < Time.time) {
            model.LastAttackTime = Time.time;
            combatSystem.ApplyDirectDamage(model.CombatId, targetCombatId, model.Config.damage);
            view.ShowDirectFrontAttack(model.Id);
        }
    }

    public InfantryState GetInfantryState(int infantryId) {
        var model = registry[infantryId];
        return new InfantryState {
            position = model.Position,
            movementVelocity = model.Velocity,
            maxSpeed = model.Config.agentAvoidanceConfig.maxSpeed,
            isAlive = !model.IsDead,
            isGrounded = model.Grounded,
            combatId = model.CombatId,
            bodyId = model.BodyPhysicsId,
        };
    }

    public AgentAvoidanceConfig GetAvoidanceConfig(int infantryId) {
        return registry[infantryId].Config.agentAvoidanceConfig;
    }

    private void ClearDeadInfantry() {
        List<InfantryModel> infantryToRemove = new();
        
        foreach (var model in registry.Values) 
            if (model.IsDead && model.Grounded) 
                infantryToRemove.Add(model);
            
        foreach (var model in infantryToRemove)
            DeleteInfantry(model);
    }

    private void DeleteInfantry(InfantryModel model) {
        registry.Remove(model.Id);
        physicsService.UnregisterPhysicsEntity(model.BodyPhysicsId);
        avoidanceService.RemoveAgent(model.AvoidanceId);
        view.RemoveVisuals(model.Id);
    }

    private void UpdateMovements() {
        foreach (var model in registry.Values) {
            var rvoVelocity = avoidanceService.GetVelocity(model.AvoidanceId);
            var physicsPose = physicsService.GetEntityPose(model.BodyPhysicsId);
            var keepFlying = !model.Grounded && physicsPose.InMotion;
            var becomeGrounded = !model.Grounded && !physicsPose.InMotion;
            var keepsGrouned = model.Grounded && !physicsPose.InMotion;
            
            if (keepFlying) {
                model.Position = physicsPose.Position;
                model.Rotation = physicsPose.Rotation;
            } else if (becomeGrounded) {
                model.Grounded = true;
                model.Position = physicsService.GetGroundPosition(model.Position);
                model.Rotation = !model.IsPhysicsOnlyMovement ? Quaternion.identity : model.Rotation;
                physicsService.SetPhysicsActive(model.BodyPhysicsId, false);
            } else if (keepsGrouned && !model.IsPhysicsOnlyMovement) {
                model.Velocity = rvoVelocity;
                model.Position = model.Position += rvoVelocity * Time.deltaTime;
                if (rvoVelocity.sqrMagnitude > 0) {
                    model.Rotation = Quaternion.LookRotation(rvoVelocity.normalized, Vector3.up);
                }
            }
        }
    }

    private void ReadCombatState() {
        foreach (var model in registry.Values) {
            if (model.IsDead)
                continue;
            
            var combatOutput = combatSystem.GetCombatOutput(model.CombatId);
            if (combatOutput.wasExploded && model.Grounded) {
                model.Grounded = false;
                physicsService.UpdatePhysicsEntityPosition(model.BodyPhysicsId, model.Position);
                physicsService.SetPhysicsActive(model.BodyPhysicsId, true);
                physicsService.AddExplosionForce(model.BodyPhysicsId, combatOutput.explosionForce, combatOutput.damageSourcePosition, combatOutput.explosionRadius, 1, ForceMode.Impulse);
            }

            // base visual effects on combat effects irregarding of logic damage
            if (combatOutput.wasProjectiled || combatOutput.wasExploded) {
                view.ShowTakeHit(model.Id);
            }

            if (combatOutput.damageWasFatal) {
                model.IsDead = true;
                model.IsPhysicsOnlyMovement = true;
                rewardController.SpawnPointReward(model.Position);
                combatSystem.UnregisterAgent(model.CombatId); // TODO: keep registered, add combat system queries filters for IsAlive

                if (combatOutput.wasProjectiled && model.Grounded) {
                    view.ShowThrownAway(model.Id, combatOutput.damageSourcePosition);
                } else {
                    view.ShowDisolveDeath(model.Id);
                }
            }
        }
    }

    private void SyncPositions() {
        foreach (var model in registry.Values) {
            view.UpdateTransform(model.Id, model.Position, model.Rotation);
            avoidanceService.SetAgentPosition(model.AvoidanceId, model.Position);
            if (!model.IsDead) {
                combatSystem.UpdateAgentPosition(model.CombatId, model.Position);
            }
        }
    }

}
