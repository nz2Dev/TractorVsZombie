using System;
using System.Collections.Generic;

using UnityEngine;
using Combat;

public class InfantryController {

    private readonly InfantryView view;
    private readonly CombatSystem combatSystem;
    private readonly AvoidanceService avoidanceService;
    private readonly RagdollService ragdollService;
    private readonly RaycastService raycastService;
    private readonly CollisionService collisionService;
    private readonly ProximityService proximityService;
    private readonly RewardController rewardController;
    private readonly InteractionRegistry interactionRegistry;
    private readonly EntityMapping entityMapping;

    private int idCounter;
    private readonly Dictionary<int, InfantryModel> registry = new();

    public InfantryController(CombatSystem combatSystem, InfantryView view, RewardController rewardController, RagdollService physicsService, RaycastService raycastService, AvoidanceService avoidanceService, ProximityService proximityService, InteractionRegistry interactionRegistry, EntityMapping entityMapping, CollisionService collisionService) {
        this.combatSystem = combatSystem;
        this.view = view;
        this.rewardController = rewardController;
        this.ragdollService = physicsService;
        this.raycastService = raycastService;
        this.avoidanceService = avoidanceService;
        this.proximityService = proximityService;
        this.interactionRegistry = interactionRegistry;
        this.entityMapping = entityMapping;
        this.collisionService = collisionService;
    }

    public int InfantryCount => registry.Count;
    public bool IsExist(int infantryId) => registry.ContainsKey(infantryId);

    public void Update() {
        UpdateMovement();
        UpdateAttacks();
        ClearDeadInfantry();
        ReadCombatState();
        SyncPositions();
    }

    public int SpawnInfantry(InfantryPrototype prototype) {
        var nextId = ++idCounter;
        var model = new InfantryModel(nextId, prototype.config, prototype.agentAvoidanceConfig.maxSpeed, prototype.rewardPrototype);
        registry[model.Id] = model;

        model.Position = prototype.position;
        model.MoveDestination = prototype.position;
        model.CombatId = combatSystem.Add(prototype.combatPrototype);
        model.CombatIsAlie = prototype.combatPrototype.alie;
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

    public void MoveTo(int infantryId, Vector3 destination) {
        registry[infantryId].MoveDestination = destination;
    }

    public void Attack(int infantryId, ProximityId targetProximityId) {
        registry[infantryId].TargetProximityId = targetProximityId;
    }

    public void ClearAttackTarget(int infantryId) {
        registry[infantryId].TargetProximityId = null;
    }

    public InfantryState GetInfantryState(int infantryId) {
        var model = registry[infantryId];
        return new InfantryState (
            position: model.Position,
            movementVelocity: model.Velocity,
            maxSpeed: model.MaxSpeed,
            activationRadius: model.Config.activationRadius,
            isAlive: !model.IsDead,
            isGrounded: model.Grounded,
            combatId: model.CombatId,
            combatIsAlie: model.CombatIsAlie,
            bodyId: model.BodyPhysicsId,
            interactionId: model.InteractionId,
            attackActivated: model.AttackActivationTime > model.LastAttackTime
        );
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

    private void UpdateMovement() {
        foreach (var model in registry.Values) {
            var rvoVelocity = avoidanceService.GetVelocity(model.AvoidanceId);
            var physicsPose = ragdollService.GetEntityPose(model.BodyPhysicsId);

            if (model.OnTheFloor && !physicsPose.ContactWithGround) {
                model.OnTheFloor = false;
            } else if (!model.OnTheFloor && physicsPose.ContactWithGround) {
                model.OnTheFloor = true;
                model.ContactWithGroundStartTime = Time.time;
            }

            var inMotion = physicsPose.Velocity.sqrMagnitude > model.Config.settleSpeedSquaredThreashold;
            var minUnsettleTimeReached = Time.time - model.UnsettleStartTime > model.Config.minUnsettleTimeSec;
            var maxTimeOnTheFloorReached = Time.time - model.ContactWithGroundStartTime > model.Config.maxTimeOnTheFloor;
            var settled = !inMotion && minUnsettleTimeReached || model.OnTheFloor && maxTimeOnTheFloorReached;
            
            var keepFlying = !model.Grounded && !settled;
            var becomeGrounded = !model.Grounded && settled;
            var keepsGrouned = model.Grounded && settled;

            if (keepFlying) {
                model.Position = physicsPose.Position;
                model.Rotation = physicsPose.Rotation;
            } else if (becomeGrounded) {
                model.Grounded = true; // todo: "Grounded", doesn't really reflect the state it represent. It's currently more like "Stable on the ground/ Stays on feet"
                model.Position = !model.IsPhysicsOnlyMovement ? collisionService.GetClosestVerticalGroundPoint(model.Position) : model.Position;
                model.Rotation = !model.IsPhysicsOnlyMovement ? Quaternion.identity : model.Rotation;
                if (!model.IsPhysicsOnlyMovement) {
                    ragdollService.SetPhysicsActive(model.BodyPhysicsId, false);
                }
            } else if (keepsGrouned && !model.IsPhysicsOnlyMovement) {
                model.Velocity = rvoVelocity;
                model.Position = model.Position += rvoVelocity * Time.deltaTime;
                if (rvoVelocity.sqrMagnitude > 0) {
                    model.Rotation = Quaternion.LookRotation(rvoVelocity.normalized, Vector3.up);
                }
            } else if (keepsGrouned && model.IsPhysicsOnlyMovement) {
                model.Position = physicsPose.Position;
                model.Rotation = physicsPose.Rotation;
            }

            var interactions = interactionRegistry.Read(model.InteractionId);
            if (interactions.activeEffect == EffectType.Explosion) {
                model.Grounded = false;
                model.UnsettleStartTime = Time.time;
                var explosion = interactions.explosionData;
                ragdollService.SetPhysicsActive(model.BodyPhysicsId, true);
                ragdollService.UpdatePhysicsEntityPosition(model.BodyPhysicsId, model.Position);
                ragdollService.AddExplosionForce(model.BodyPhysicsId, explosion.config.force, explosion.epicentr, 
                    explosion.config.radius, explosion.config.upwardModifier, explosion.config.forceMode);
            }

            
        }
    }

    private void UpdateAttacks() {
        foreach (var model in registry.Values) {
            if (model.IsDead)
                continue;

            var attackCanceled = !model.TargetProximityId.HasValue || !model.Grounded;
            if (attackCanceled) {
                model.AttackActivationTime = model.LastAttackTime - 1;
                view.ShowDischarge(model.Id);
                continue;
            }

            var targetProximityId = model.TargetProximityId.Value;
            var lostAbilityForCombat = !entityMapping.TryFindByProximityId(targetProximityId, out var targetComponents) 
                || !targetComponents.combatId.HasValue;
            if (lostAbilityForCombat) {
                model.TargetProximityId = null;
                model.AttackActivationTime = model.LastAttackTime - 1;
                view.ShowDischarge(model.Id);
                continue;
            }

            var targetPosition = proximityService.GetPoint(targetProximityId);
            var targetDirection = (targetPosition - model.Position).normalized;
            model.Rotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            var canActivate = model.AttackActivationTime <= model.LastAttackTime;
            if (canActivate && model.LastAttackTime + model.Config.attackCooldown < Time.time) {
                model.AttackActivationTime = Time.time;
                model.AttackPosition = model.Position;
                view.ShowCharge(model.Id, model.Config.attackDuration);
            }

            var charging = Time.time >= model.AttackActivationTime && Time.time < model.AttackActivationTime + model.Config.attackDuration;
            if (charging) {
                model.Position = model.AttackPosition;
                model.MoveDestination = model.AttackPosition;
            }

            var notActivated = model.AttackActivationTime < model.LastAttackTime;
            var notChargedYet = Time.time < model.AttackActivationTime + model.Config.attackDuration;
            if (notActivated || notChargedYet) {
                continue;
            }
            
            model.LastAttackTime = Time.time;
            view.ResetCharge(model.Id);
            
            var targetRaycastState = raycastService.ReadState(targetComponents.raycastId.Value);
            var outOfReach = Vector3.Distance(model.Position, targetPosition) > model.Config.activationRadius + targetRaycastState.radius;
            if (outOfReach) {
                continue;
            }
            
            view.ShowDirectFrontAttack(model.Id, targetPosition);
            combatSystem.DealDamage(targetComponents.combatId.Value, new DamageInput {
                damageSource = model.Position,
                damageType = DamageType.Punch,
                damage = model.Config.damage
            });
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
            view.UpdateTransform(model.Id, model.Position, model.Rotation, model.Velocity.magnitude / model.MaxSpeed);
            proximityService.UpdatePoint(model.ProximityId, model.Position);
            raycastService.UpdateMarker(model.RaycastId, model.Position);
            // if (!model.IsDead) {
            //     combatSystem.UpdateAgentPosition(model.CombatId, model.Position);
            // }

            if (model.Grounded) {
                avoidanceService.SetAgentPosition(model.AvoidanceId, model.Position);
                var moveVelocity = model.MoveDestination - model.Position;
                var distance = moveVelocity.magnitude;
                var speedFactor = Mathf.Clamp01(distance / model.Config.stoppingDistance);
                var velocity = distance > float.Epsilon ? moveVelocity / distance * model.MaxSpeed * speedFactor : Vector3.zero;
                avoidanceService.SetPreferedVelocity(model.AvoidanceId, velocity);
            }
        }
    }

}
