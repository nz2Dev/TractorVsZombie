using System;
using System.Collections.Generic;

using UnityEngine;
using Combat;

public class InfantryController {

    private readonly InfantryView view;
    private readonly CombatSystem combatSystem;
    private readonly AvoidanceService avoidanceService;
    private readonly MotionSystem motionSystem;
    private readonly RaycastService raycastService;
    private readonly ProximityService proximityService;
    private readonly RewardController rewardController;
    private readonly EntityMapping entityMapping;

    private int idCounter;
    private readonly Dictionary<int, InfantryModel> registry = new();

    public InfantryController(CombatSystem combatSystem, InfantryView view, RewardController rewardController, MotionSystem motionSystem, RaycastService raycastService, AvoidanceService avoidanceService, ProximityService proximityService, EntityMapping entityMapping) {
        this.combatSystem = combatSystem;
        this.view = view;
        this.rewardController = rewardController;
        this.motionSystem = motionSystem;
        this.raycastService = raycastService;
        this.avoidanceService = avoidanceService;
        this.proximityService = proximityService;
        this.entityMapping = entityMapping;
    }

    public int InfantryCount => registry.Count;
    public bool IsExist(int infantryId) => registry.ContainsKey(infantryId);

    public void Update() {
        ReadComponents();
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
        model.MotionId = motionSystem.Add(prototype.position, prototype.config, prototype.physicsBodyPrefab);
        model.AvoidanceId = avoidanceService.AddAgent(prototype.position, prototype.agentAvoidanceConfig);
        model.ProximityId = proximityService.AddPoint(prototype.position, CombatSystem.GetProximityLayerForFaction(prototype.combatPrototype.alie));
        model.RaycastId = raycastService.RegisterMarker(prototype.position, prototype.raycastMarkerPrefab, CombatSystem.GetRaycastLayerForFaction(prototype.combatPrototype.alie));

        entityMapping.CreateMappings(new EntityComponents {
            proximityId = model.ProximityId,
            raycastId = model.RaycastId,
            combatId = model.CombatId,
            motionId = model.MotionId
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
            isGrounded: model.MotionState.isGrounded,
            combatId: model.CombatId,
            combatIsAlie: model.CombatIsAlie,
            attackActivated: model.AttackActivationTime > model.LastAttackTime
        );
    }

    private void ClearDeadInfantry() {
        List<InfantryModel> infantryToRemove = new();

        foreach (var model in registry.Values)
            if (model.IsDead && model.MotionState.isGrounded)
                infantryToRemove.Add(model);

        foreach (var model in infantryToRemove)
            DeleteInfantry(model);
    }

    private void DeleteInfantry(InfantryModel model) {
        registry.Remove(model.Id);
        
        combatSystem.Remove(model.CombatId);
        motionSystem.Remove(model.MotionId);
        avoidanceService.RemoveAgent(model.AvoidanceId);
        proximityService.RemovePoint(model.ProximityId);
        raycastService.UnregisterMarker(model.RaycastId);

        entityMapping.DeleteMappings(model.ProximityId, model.RaycastId);

        view.RemoveVisuals(model.Id);
    }

    private void UpdateMovement() {
        foreach (var model in registry.Values) {
            var motionState = model.MotionState;
            var rvoVelocity = avoidanceService.GetVelocity(model.AvoidanceId);

            if (motionState.becameGrounded) {
                model.Position = motionState.position;
                model.Rotation = motionState.rotation;
            }

            if (!motionState.isGrounded || model.IsMotionOnlyMovement) {
                model.Position = motionState.position;
                model.Rotation = motionState.rotation;
            } else {
                model.Velocity = rvoVelocity;
                model.Position += rvoVelocity * Time.deltaTime;
                if (rvoVelocity.sqrMagnitude < float.Epsilon) {
                    model.Rotation = Quaternion.identity;
                } else {
                    model.Rotation = Quaternion.LookRotation(rvoVelocity.normalized, Vector3.up);
                }
            }

            
        }
    }

    private void UpdateAttacks() {
        foreach (var model in registry.Values) {
            if (model.IsDead)
                continue;

            var attackCanceled = !model.TargetProximityId.HasValue || !model.MotionState.isGrounded;
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
            view.PlayChargeAttack(model.Id);
            
            var targetRaycastState = raycastService.ReadState(targetComponents.raycastId.Value);
            var outOfReach = Vector3.Distance(model.Position, targetPosition) > model.Config.activationRadius + targetRaycastState.radius;
            if (outOfReach) {
                continue;
            }
            
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
                    model.IsMotionOnlyMovement = true;
                    // keep awake not enough, need aditional state for "settled", or option to prolongue unsettled state
                    // as motion state fethcing and removal depend on grounded state, and keeping rigidbody always awake has no effect yet.
                    motionSystem.KeepAwake(model.MotionId);
                    rewardController.Create(model.RewardPrototype, model.Position);
                    
                    if (damageResult.damageType == DamageType.Projectile && model.MotionState.isGrounded) {
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
            motionSystem.SetPose(model.MotionId, model.Position, model.Rotation);
            view.UpdateTransform(model.Id, model.Position, model.Rotation, model.Velocity.magnitude / model.MaxSpeed);
            proximityService.UpdatePoint(model.ProximityId, model.Position);
            raycastService.UpdateMarker(model.RaycastId, model.Position);
            // if (!model.IsDead) {
            //     combatSystem.UpdateAgentPosition(model.CombatId, model.Position);
            // }

            if (model.MotionState.isGrounded) {
                avoidanceService.SetAgentPosition(model.AvoidanceId, model.Position);
                var moveVelocity = model.MoveDestination - model.Position;
                var distance = moveVelocity.magnitude;
                var speedFactor = Mathf.Clamp01(distance / model.Config.stoppingDistance);
                var velocity = distance > float.Epsilon ? moveVelocity / distance * model.MaxSpeed * speedFactor : Vector3.zero;
                avoidanceService.SetPreferedVelocity(model.AvoidanceId, velocity);
            }
        }
    }

    private void ReadComponents() {
        foreach (var model in registry.Values) {
            model.MotionState = motionSystem.ReadState(model.MotionId);
        }
    }

}
