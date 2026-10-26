using System;
using System.Collections.Generic;

using UnityEngine;

public class InfantryController {

    private readonly InfantryView view;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly MotionSystem motionSystem;
    private readonly MovementSystem movementSystem;
    private readonly CombatSystem combatSystem;
    private readonly HitboxSystem hitboxSystem;
    private readonly DropSystem dropSystem;
    private readonly EntityMapping entityMapping;
    private readonly PoseRegistry poseRegistry;

    private int idCounter;
    private readonly Dictionary<int, InfantryModel> registry = new();

    public InfantryController(HealthSystem healthSystem, FactionRegistry factionRegistry, InfantryView view, DropSystem dropSystem, MotionSystem motionSystem, MovementSystem movementSystem, CombatSystem combatSystem, HitboxSystem hitboxSystem, EntityMapping entityMapping, PoseRegistry poseRegistry) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.view = view;
        this.dropSystem = dropSystem;
        this.motionSystem = motionSystem;
        this.movementSystem = movementSystem;
        this.combatSystem = combatSystem;
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
        ClearDeadInfantry();
        ReadHealthState();
        UpdatePresentation();
    }

    public int SpawnInfantry(InfantryPrototype prototype) {
        var nextId = ++idCounter;
        var model = new InfantryModel(nextId);
        registry[model.Id] = model;

        model.PoseId = poseRegistry.Add(prototype.position, default);
        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        model.MotionId = motionSystem.Add(model.PoseId, prototype.position, prototype.motionPrototype, prototype.physicsBodyPrefab, model.HealthId);
        model.MovementId = movementSystem.Add(prototype.position, model.MotionId, model.PoseId, prototype.movementPrototype);
        model.CombatId = combatSystem.Add(prototype.combatPrototype, model.HealthId, model.MotionId, model.MovementId, model.PoseId);
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
        combatSystem.SetTarget(registry[infantryId].CombatId, targetHitboxId);
    }

    public void StopAttack(int infantryId) {
        combatSystem.ClearTarget(registry[infantryId].CombatId);
    }

    public InfantryState GetInfantryState(int infantryId) {
        var model = registry[infantryId];
        var combatState = combatSystem.ReadState(model.CombatId);
        return new InfantryState (
            position: model.PoseState.position,
            maxSpeed: model.MovementState.maxSpeed,
            activationRadius: combatState.activationRadius,
            isAlive: !model.HealthState.isDead,
            isGrounded: model.MotionState.isGrounded,
            healthId: model.HealthId,
            factionIsAlie: factionRegistry.ReadState(model.FactionId).alie,
            attackActivated: combatState.attackActivated
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
        combatSystem.Remove(model.CombatId);
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

    private void UpdatePresentation() {
        foreach (var model in registry.Values) {
            var pose = poseRegistry.Read(model.PoseId);
            view.UpdateTransform(model.Id, pose.position, pose.rotation, model.MovementState.velocity.magnitude / model.MovementState.maxSpeed);

            var combatState = combatSystem.ReadState(model.CombatId);
            switch (combatState.attackEvent) {
                case CombatEvent.ChargeActivated:
                    view.ShowCharge(model.Id, combatState.attackDuration);
                    break;
                case CombatEvent.AttackActivated:
                    view.PlayChargeAttack(model.Id);
                    break;
                case CombatEvent.Canceled:
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
