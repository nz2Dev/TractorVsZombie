using System;
using System.Collections.Generic;

using UnityEngine;

public class InfantryController {

    private readonly InfantryView view;
    private readonly CombatSystem combatSystem;
    private readonly LocalAvoidanceService avoidanceService;
    private readonly RagdollService ragdollService;
    private readonly RaycastService raycastService;
    private readonly RewardController rewardController;

    private int idCounter;
    private readonly Dictionary<int, InfantryModel> registry = new();

    public InfantryController(CombatSystem combatSystem, InfantryView view, RewardController rewardController, RagdollService physicsService, RaycastService raycastService, LocalAvoidanceService avoidanceService) {
        this.combatSystem = combatSystem;
        this.view = view;
        this.rewardController = rewardController;
        this.ragdollService = physicsService;
        this.raycastService = raycastService;
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

    public int SpawnInfantry(InfantryPrototype prototype) {
        var nextId = ++idCounter;
        var model = new InfantryModel(nextId, prototype.config);
        registry[model.Id] = model;
        
        model.Position = prototype.position;
        model.CombatId = combatSystem.RegisterAgent(prototype.position, prototype.combatAgentPrototype);
        model.BodyPhysicsId = ragdollService.RegisterPhysicsEntity(prototype.position, prototype.physicsBodyPrefab);
        model.AvoidanceId = avoidanceService.AddAgent(prototype.position, model.Config.agentAvoidanceConfig);
        model.RewardPrototype = prototype.rewardPrototype;

        view.AddVisuals(model.Id, prototype.position, prototype.visualsPrefab);
        return model.Id;
    }

    public void Move(int infantryId, Vector3 velocity) {
        var model = registry[infantryId];
        avoidanceService.SetPreferedVelocity(model.AvoidanceId, velocity);
    }

    public void Position(int infantryId, Vector3 position) {
        var model = registry[infantryId];
        model.Position = position;
        ragdollService.UpdatePhysicsEntityPosition(model.BodyPhysicsId, position);
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
        ragdollService.UnregisterPhysicsEntity(model.BodyPhysicsId);
        avoidanceService.RemoveAgent(model.AvoidanceId);
        view.RemoveVisuals(model.Id);
    }

    private void UpdateMovements() {
        foreach (var model in registry.Values) {
            var rvoVelocity = avoidanceService.GetVelocity(model.AvoidanceId);
            var physicsPose = ragdollService.GetEntityPose(model.BodyPhysicsId);
            var keepFlying = !model.Grounded && physicsPose.InMotion;
            var becomeGrounded = !model.Grounded && !physicsPose.InMotion;
            var keepsGrouned = model.Grounded && !physicsPose.InMotion;
            
            if (keepFlying) {
                model.Position = physicsPose.Position;
                model.Rotation = physicsPose.Rotation;
            } else if (becomeGrounded) {
                model.Grounded = true;
                model.Position = raycastService.GetClosestVerticalGroundPoint(model.Position);
                model.Rotation = !model.IsPhysicsOnlyMovement ? Quaternion.identity : model.Rotation;
                ragdollService.SetPhysicsActive(model.BodyPhysicsId, false);
                combatSystem.RecoverFromExplosion(model.CombatId);
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
                ragdollService.UpdatePhysicsEntityPosition(model.BodyPhysicsId, model.Position);
                ragdollService.SetPhysicsActive(model.BodyPhysicsId, true);
                var explosion = combatOutput.explosionData;
                ragdollService.AddExplosionForce(model.BodyPhysicsId, explosion.force, combatOutput.damageSourcePosition, explosion.radius, explosion.upwardModifier, ForceMode.Impulse);
            }

            // base visual effects on combat effects irregarding of logic damage
            if (combatOutput.wasProjectiled || combatOutput.wasExploded) {
                view.ShowTakeHit(model.Id);
            }

            if (combatOutput.damageWasFatal) {
                model.IsDead = true;
                model.IsPhysicsOnlyMovement = true;
                rewardController.Create(model.RewardPrototype, model.Position);
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
