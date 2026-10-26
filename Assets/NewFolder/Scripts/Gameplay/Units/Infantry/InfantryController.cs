using System;
using System.Collections.Generic;

using UnityEngine;

public class InfantryController {

    private readonly InfantryView view;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly MotionSystem motionSystem;
    private readonly MovementSystem movementSystem;
    private readonly HitboxSystem hitboxSystem;
    private readonly DropSystem dropSystem;
    private readonly EntityMapping entityMapping;
    private readonly PoseRegistry poseRegistry;

    private int idCounter;
    private readonly Dictionary<int, InfantryModel> registry = new();

    public InfantryController(HealthSystem healthSystem, FactionRegistry factionRegistry, InfantryView view, DropSystem dropSystem, MotionSystem motionSystem, MovementSystem movementSystem, HitboxSystem hitboxSystem, EntityMapping entityMapping, PoseRegistry poseRegistry) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.view = view;
        this.dropSystem = dropSystem;
        this.motionSystem = motionSystem;
        this.movementSystem = movementSystem;
        this.hitboxSystem = hitboxSystem;
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
        ReadHealthState();
        UpdatePresentation();
    }

    public int SpawnInfantry(InfantryPrototype prototype) {
        var nextId = ++idCounter;
        var model = new InfantryModel(nextId, prototype.config);
        registry[model.Id] = model;

        model.PoseId = poseRegistry.Add(prototype.position, default);
        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        model.MotionId = motionSystem.Add(model.PoseId, prototype.position, prototype.config, prototype.physicsBodyPrefab, model.HealthId);
        model.MovementId = movementSystem.Add(prototype.position, prototype.agentAvoidanceConfig, model.MotionId, model.PoseId, prototype.config);
        model.HitboxId = hitboxSystem.Add(model.FactionId, model.PoseId, prototype.hitboxPrototype);
        model.DropId = dropSystem.Add(prototype.dropPrototype, model.HealthId, model.PoseId);

        entityMapping.CreateMapping(model.HitboxId, new EntityComponents {
            healthId = model.HealthId,
            factionId = model.FactionId,
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

    public void Attack(int infantryId, HitboxId targetHitboxId) {
        registry[infantryId].TargetHitboxId = targetHitboxId;
    }

    public void StopAttack(int infantryId) {
        registry[infantryId].TargetHitboxId = null;
    }

    public InfantryState GetInfantryState(int infantryId) {
        var model = registry[infantryId];
        return new InfantryState (
            position: model.PoseState.position,
            maxSpeed: model.MovementState.maxSpeed,
            activationRadius: model.Config.activationRadius,
            isAlive: !model.HealthState.isDead,
            isGrounded: model.MotionState.isGrounded,
            healthId: model.HealthId,
            factionIsAlie: factionRegistry.ReadState(model.FactionId).alie,
            attackActivated: model.AttackActivationTime > model.LastAttackTime
        );
    }

    private void ClearDeadInfantry() {
        List<InfantryModel> infantryToRemove = new();

        foreach (var model in registry.Values)
            if (model.HealthState.isDead && model.MotionState.isGrounded)
                infantryToRemove.Add(model);

        foreach (var model in infantryToRemove)
            DeleteInfantry(model);
    }

    private void DeleteInfantry(InfantryModel model) {
        registry.Remove(model.Id);

        healthSystem.Remove(model.HealthId);
        movementSystem.Remove(model.MovementId);
        motionSystem.Remove(model.MotionId);
        poseRegistry.Remove(model.PoseId);
        hitboxSystem.Remove(model.HitboxId);
        dropSystem.Remove(model.DropId);

        entityMapping.DeleteMapping(model.HitboxId);
        factionRegistry.Remove(model.FactionId);

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

    private void ReadHealthState() {
        foreach (var model in registry.Values) {
            model.HealthState = healthSystem.ReadState(model.HealthId);
        }
    }

    private void UpdateAttacks() {
        foreach (var model in registry.Values) {
            model.AttackEvents = ChargeAttackEvents.None;
            if (model.HealthState.isDead)
                continue;

            var attackCanceled = !model.TargetHitboxId.HasValue || !model.MotionState.isGrounded;
            if (attackCanceled) {
                model.AttackActivationTime = model.LastAttackTime - 1;
                model.AttackEvents = ChargeAttackEvents.Canceled;
                continue;
            }

            var targetHitboxId = model.TargetHitboxId.Value;
            var lostAbilityForTarget = !entityMapping.TryFindByHitboxId(targetHitboxId, out var targetComponents)
                || !targetComponents.healthId.HasValue;
            if (lostAbilityForTarget) {
                model.TargetHitboxId = null;
                model.AttackActivationTime = model.LastAttackTime - 1;
                model.AttackEvents = ChargeAttackEvents.Canceled;
                continue;
            }

            var pose = poseRegistry.Read(model.PoseId);
            var targetPosition = hitboxSystem.GetPosition(targetHitboxId);
            var targetDirection = (targetPosition - pose.position).normalized;
            pose.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            poseRegistry.WriteRotation(model.PoseId, pose.rotation);

            var canActivate = model.AttackActivationTime <= model.LastAttackTime;
            if (canActivate && model.LastAttackTime + model.Config.attackCooldown < Time.time) {
                model.AttackActivationTime = Time.time;
                model.AttackEvents = ChargeAttackEvents.ChargeActivated;
            }

            var charging = Time.time > model.AttackActivationTime && Time.time < model.AttackActivationTime + model.Config.attackDuration;
            if (charging) {
                movementSystem.HoldMovement(model.MovementId);
            }

            var notActivated = model.AttackActivationTime < model.LastAttackTime;
            var notChargedYet = Time.time < model.AttackActivationTime + model.Config.attackDuration;
            if (notActivated || notChargedYet) {
                continue;
            }

            model.LastAttackTime = Time.time;
            model.AttackEvents = ChargeAttackEvents.AttackActivated;

            var targetHitboxState = hitboxSystem.ReadState(targetHitboxId);
            var outOfReach = Vector3.Distance(pose.position, targetPosition) > model.Config.activationRadius + targetHitboxState.radius;
            if (outOfReach) {
                continue;
            }

            healthSystem.DealDamage(targetComponents.healthId.Value, new DamageInput {
                damageSource = pose.position,
                damageType = DamageType.Punch,
                damage = model.Config.damage
            });
        }
    }

    private void UpdatePresentation() {
        foreach (var model in registry.Values) {
            var pose = poseRegistry.Read(model.PoseId);
            view.UpdateTransform(model.Id, pose.position, pose.rotation, model.MovementState.velocity.magnitude / model.MovementState.maxSpeed);

            switch (model.AttackEvents) {
                case ChargeAttackEvents.ChargeActivated:
                    view.ShowCharge(model.Id, model.Config.attackDuration);
                    break;
                case ChargeAttackEvents.AttackActivated:
                    view.PlayChargeAttack(model.Id);
                    break;
                case ChargeAttackEvents.Canceled:
                    view.ShowDischarge(model.Id);
                    break;
                default:
                    break;
            }

            var healthState = model.HealthState;
            var healthNormalized = 1f - (float) healthState.health / Mathf.Max(healthState.maxHealth, 1);
            view.SetHealthNormalized(model.Id, healthNormalized);

            if (healthState.damageResult.HasValue) {
                view.ShowTakeHit(model.Id);

                var damageResult = healthState.damageResult.Value;
                if (damageResult.damageWasFatal) {
                    if (damageResult.damageType == DamageType.Projectile && model.MotionState.isGrounded) {
                        view.ShowThrownAway(model.Id, damageResult.damageSource);
                    } else {
                        view.ShowDisolveDeath(model.Id);
                    }
                }
            }
        }
    }

}
