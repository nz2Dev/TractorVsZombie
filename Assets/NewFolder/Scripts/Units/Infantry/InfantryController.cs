using System;
using System.Collections.Generic;

using UnityEngine;
using Combat;

public class InfantryController {

    private readonly InfantryView view;
    private readonly CombatSystem combatSystem;
    private readonly LocalAvoidanceService avoidanceService;
    private readonly RagdollService ragdollService;
    private readonly RaycastService raycastService;
    private readonly ProximityService proximityService;
    private readonly RewardController rewardController;
    private readonly InteractionRegistry interactionRegistry;
    private readonly EntityMapping entityMapping;

    private int idCounter;
    private readonly Dictionary<int, InfantryModel> registry = new();
    
    public InfantryController(CombatSystem combatSystem, InfantryView view, RewardController rewardController, RagdollService physicsService, RaycastService raycastService, LocalAvoidanceService avoidanceService, ProximityService proximityService, InteractionRegistry interactionRegistry, EntityMapping entityMapping) {
        this.combatSystem = combatSystem;
        this.view = view;
        this.rewardController = rewardController;
        this.ragdollService = physicsService;
        this.raycastService = raycastService;
        this.avoidanceService = avoidanceService;
        this.proximityService = proximityService;
        this.interactionRegistry = interactionRegistry;
        this.entityMapping = entityMapping;
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
        var model = new InfantryModel(nextId, prototype.config, prototype.agentAvoidanceConfig.maxSpeed, prototype.rewardPrototype);
        registry[model.Id] = model;

        model.Position = prototype.position;
        model.CombatId = combatSystem.Add(prototype.combatPrototype);
        model.InteractionId = interactionRegistry.Add();
        model.BodyPhysicsId = ragdollService.RegisterPhysicsEntity(prototype.position, prototype.physicsBodyPrefab);
        model.AvoidanceId = avoidanceService.AddAgent(prototype.position, prototype.agentAvoidanceConfig);
        model.ProximityId = proximityService.AddPoint(prototype.position, CombatSystem.GetProximityLayerForFaction(prototype.combatPrototype.alie));
        model.RaycastId = raycastService.RegisterMarker(prototype.position, prototype.raycastMarkerPrefab, CombatSystem.GetRaycastLayerForFaction(prototype.combatPrototype.alie));

        entityMapping.CreateMappings(new EntityComponents {
            proximityId = model.ProximityId,
            raycastId = model.RaycastId,
            combatId = model.CombatId,
            interactionId = model.InteractionId
        });

        view.AddVisuals(model.Id, prototype.position, prototype.visualsPrefab);
        return model.Id;
    }

    public void Move(int infantryId, Vector3 velocity) {
        var model = registry[infantryId];
        avoidanceService.SetPreferedVelocity(model.AvoidanceId, velocity);
    }

    public void Attack(int infantryId, CombatId targetCombatId) {
        var model = registry[infantryId];
        if (model.LastAttackTime + model.Config.attackCooldown < Time.time) {
            model.LastAttackTime = Time.time;
            view.ShowDirectFrontAttack(model.Id);
            combatSystem.DealDamage(targetCombatId, new DamageInput {
                damageSource = model.Position,
                damageType = DamageType.Punch,
                damage = model.Config.damage
            });
        }
    }

    public InfantryState GetInfantryState(int infantryId) {
        var model = registry[infantryId];
        return new InfantryState {
            position = model.Position,
            movementVelocity = model.Velocity,
            maxSpeed = model.MaxSpeed,
            isAlive = !model.IsDead,
            isGrounded = model.Grounded,
            combatId = model.CombatId,
            bodyId = model.BodyPhysicsId,
            interactionId = model.InteractionId,
        };
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
        
        combatSystem.Remove(model.CombatId);
        interactionRegistry.Remove(model.InteractionId);
        ragdollService.UnregisterPhysicsEntity(model.BodyPhysicsId);
        avoidanceService.RemoveAgent(model.AvoidanceId);
        proximityService.RemovePoint(model.ProximityId);
        raycastService.UnregisterMarker(model.RaycastId);

        entityMapping.DeleteMappings(model.ProximityId, model.RaycastId);

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
                model.Grounded = true; // todo: "Grounded", doesn't really reflect the state it represent. It's currently more like "Stable on the ground/ Stays on feet"
                model.Position = raycastService.GetClosestVerticalGroundPoint(model.Position);
                model.Rotation = !model.IsPhysicsOnlyMovement ? Quaternion.identity : model.Rotation;
                ragdollService.SetPhysicsActive(model.BodyPhysicsId, false);
                model.ExplosionForbiden = false;
            } else if (keepsGrouned && !model.IsPhysicsOnlyMovement) {
                model.Velocity = rvoVelocity;
                model.Position = model.Position += rvoVelocity * Time.deltaTime;
                if (rvoVelocity.sqrMagnitude > 0) {
                    model.Rotation = Quaternion.LookRotation(rvoVelocity.normalized, Vector3.up);
                }
            }

            var interactions = interactionRegistry.Read(model.InteractionId);
            if (interactions.activeEffect == EffectType.Explosion) {
                model.Grounded = false;
                var explosion = interactions.explosionData;
                ragdollService.SetPhysicsActive(model.BodyPhysicsId, true);
                ragdollService.UpdatePhysicsEntityPosition(model.BodyPhysicsId, model.Position);
                ragdollService.AddExplosionForce(model.BodyPhysicsId, explosion.config.force, explosion.epicentr, 
                    explosion.config.radius, explosion.config.upwardModifier, ForceMode.Impulse);
            }
        }
    }

    private void ReadCombatState() {
        foreach (var model in registry.Values) {
            if (model.IsDead)
                continue;

            var combatState = combatSystem.ReadState(model.CombatId);
            if (combatState.damageResult.HasValue) {
                view.ShowTakeHit(model.Id);
            }

            if (combatState.damageResult.HasValue) {
                var damageResult = combatState.damageResult.Value;
                if (damageResult.damageWasFatal) {
                    model.IsDead = true;
                    model.IsPhysicsOnlyMovement = true;
                    rewardController.Create(model.RewardPrototype, model.Position);
                    
                    if (damageResult.damageType == DamageType.Projectile && model.Grounded) {
                        view.ShowThrownAway(model.Id, damageResult.damageSource);
                    } else {
                        view.ShowDisolveDeath(model.Id);
                    }
                }
            }
        }
    }

    private void SyncPositions() {
        foreach (var model in registry.Values) {
            view.UpdateTransform(model.Id, model.Position, model.Rotation);
            avoidanceService.SetAgentPosition(model.AvoidanceId, model.Position);
            proximityService.UpdatePoint(model.ProximityId, model.Position);
            raycastService.UpdateMarker(model.RaycastId, model.Position);
            // if (!model.IsDead) {
            //     combatSystem.UpdateAgentPosition(model.CombatId, model.Position);
            // }
        }
    }

}
