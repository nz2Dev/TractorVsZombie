using System;
using System.Collections.Generic;

using UnityEngine;
using Combat;

public class InfantryController {

    private readonly InfantryView view;
    private readonly CombatSystem combatSystem;
    private readonly MotionSystem motionSystem;
    private readonly MovementSystem movementSystem;
    private readonly RaycastService raycastService;
    private readonly ProximityService proximityService;
    private readonly RewardController rewardController;
    private readonly EntityMapping entityMapping;
    private readonly PoseRegistry poseRegistry;

    private int idCounter;
    private readonly Dictionary<int, InfantryModel> registry = new();

    public InfantryController(CombatSystem combatSystem, InfantryView view, RewardController rewardController, MotionSystem motionSystem, MovementSystem movementSystem, RaycastService raycastService, ProximityService proximityService, EntityMapping entityMapping, PoseRegistry poseRegistry) {
        this.combatSystem = combatSystem;
        this.view = view;
        this.rewardController = rewardController;
        this.motionSystem = motionSystem;
        this.movementSystem = movementSystem;
        this.raycastService = raycastService;
        this.proximityService = proximityService;
        this.entityMapping = entityMapping;
        this.poseRegistry = poseRegistry;
    }

    public int InfantryCount => registry.Count;
    public bool IsExist(int infantryId) => registry.ContainsKey(infantryId);

    public void Update() {
        ReadPose();
        ReadMotion();
        ReadMovement();
        UpdateAttacks();
        ClearDeadInfantry();
        ReadCombatState();
        SyncPositions();
    }

    public int SpawnInfantry(InfantryPrototype prototype) {
        var nextId = ++idCounter;
        var model = new InfantryModel(nextId, prototype.config, prototype.rewardPrototype);
        registry[model.Id] = model;

        model.PoseId = poseRegistry.Add(prototype.position, default);
        model.CombatId = combatSystem.Add(prototype.combatPrototype);
        model.CombatIsAlie = prototype.combatPrototype.alie;
        model.MotionId = motionSystem.Add(model.PoseId, prototype.position, prototype.config, prototype.physicsBodyPrefab, model.CombatId);
        model.MovementId = movementSystem.Add(prototype.position, prototype.agentAvoidanceConfig, model.MotionId, model.PoseId, prototype.config);
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
        movementSystem.MoveTo(registry[infantryId].MovementId, destination);
    }

    public void StopMovement(int infantryId) {
        movementSystem.StopMovement(registry[infantryId].MovementId);
    }

    public void Attack(int infantryId, ProximityId targetProximityId) {
        registry[infantryId].TargetProximityId = targetProximityId;
    }

    public void StopAttack(int infantryId) {
        registry[infantryId].TargetProximityId = null;
    }

    public InfantryState GetInfantryState(int infantryId) {
        var model = registry[infantryId];
        return new InfantryState (
            position: model.PoseState.position,
            maxSpeed: model.MovementState.maxSpeed,
            activationRadius: model.Config.activationRadius,
            isAlive: !model.CombatState.isDead,
            isGrounded: model.MotionState.isGrounded,
            combatId: model.CombatId,
            combatIsAlie: model.CombatIsAlie,
            attackActivated: model.AttackActivationTime > model.LastAttackTime
        );
    }

    private void ClearDeadInfantry() {
        List<InfantryModel> infantryToRemove = new();

        foreach (var model in registry.Values)
            if (model.CombatState.isDead && model.MotionState.isGrounded)
                infantryToRemove.Add(model);

        foreach (var model in infantryToRemove)
            DeleteInfantry(model);
    }

    private void DeleteInfantry(InfantryModel model) {
        registry.Remove(model.Id);
        
        combatSystem.Remove(model.CombatId);
        movementSystem.Remove(model.MovementId);
        motionSystem.Remove(model.MotionId);
        poseRegistry.Remove(model.PoseId);
        proximityService.RemovePoint(model.ProximityId);
        raycastService.UnregisterMarker(model.RaycastId);

        entityMapping.DeleteMappings(model.ProximityId, model.RaycastId);

        view.RemoveVisuals(model.Id);
    }

    private void ReadPose() {
        foreach (var model in registry.Values) {
            model.PoseState = poseRegistry.Read(model.PoseId);
        }
    }

    private void ReadMotion() {
        foreach (var model in registry.Values) {
            model.MotionState = motionSystem.ReadState(model.MotionId);
        }
    }

    private void ReadMovement() {
        foreach (var model in registry.Values) {
            model.MovementState = movementSystem.ReadState(model.MovementId);
        }
    }

    private void UpdateAttacks() {
        foreach (var model in registry.Values) {
            if (model.CombatState.isDead)
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

            var pose = poseRegistry.Read(model.PoseId);
            var targetPosition = proximityService.GetPoint(targetProximityId);
            var targetDirection = (targetPosition - pose.position).normalized;
            pose.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            poseRegistry.WriteRotation(model.PoseId, pose.rotation);

            var canActivate = model.AttackActivationTime <= model.LastAttackTime;
            if (canActivate && model.LastAttackTime + model.Config.attackCooldown < Time.time) {
                model.AttackActivationTime = Time.time;
                view.ShowCharge(model.Id, model.Config.attackDuration);
            }

            var charging = Time.time >= model.AttackActivationTime && Time.time < model.AttackActivationTime + model.Config.attackDuration;
            if (charging) {
                movementSystem.HoldMovement(model.MovementId);
            }

            var notActivated = model.AttackActivationTime < model.LastAttackTime;
            var notChargedYet = Time.time < model.AttackActivationTime + model.Config.attackDuration;
            if (notActivated || notChargedYet) {
                continue;
            }
            
            model.LastAttackTime = Time.time;
            view.PlayChargeAttack(model.Id);
            
            var targetRaycastState = raycastService.ReadState(targetComponents.raycastId.Value);
            var outOfReach = Vector3.Distance(pose.position, targetPosition) > model.Config.activationRadius + targetRaycastState.radius;
            if (outOfReach) {
                continue;
            }
            
            combatSystem.DealDamage(targetComponents.combatId.Value, new DamageInput {
                damageSource = pose.position,
                damageType = DamageType.Punch,
                damage = model.Config.damage
            });
        }
    }

    private void ReadCombatState() {
        foreach (var model in registry.Values) {
            model.CombatState = combatSystem.ReadState(model.CombatId);
            
            var combatState = model.CombatState;
            if (!combatState.damageResult.HasValue)
                continue;

            view.ShowTakeHit(model.Id);
            
            var damageResult = combatState.damageResult.Value;
            if (damageResult.damageWasFatal) {
                var pose = poseRegistry.Read(model.PoseId);
                rewardController.Create(model.RewardPrototype, pose.position);
                
                if (damageResult.damageType == DamageType.Projectile && model.MotionState.isGrounded) {
                    view.ShowThrownAway(model.Id, damageResult.damageSource);
                } else {
                    view.ShowDisolveDeath(model.Id);
                }
            }
        }
    }

    private void SyncPositions() {
        foreach (var model in registry.Values) {
            var pose = poseRegistry.Read(model.PoseId);
            proximityService.UpdatePoint(model.ProximityId, pose.position);
            raycastService.UpdateMarker(model.RaycastId, pose.position);
            view.UpdateTransform(model.Id, pose.position, pose.rotation, model.MovementState.velocity.magnitude / model.MovementState.maxSpeed);
        }
    }

}
