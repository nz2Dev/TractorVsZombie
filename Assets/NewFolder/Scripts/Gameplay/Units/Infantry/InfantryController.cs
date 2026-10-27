using System;
using System.Collections.Generic;

using UnityEngine;

public class InfantryController {

    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly MotionSystem motionSystem;
    private readonly MovementSystem movementSystem;
    private readonly CombatSystem combatSystem;
    private readonly HitboxSystem hitboxSystem;
    private readonly DropSystem dropSystem;
    private readonly EntityMapping entityMapping;
    private readonly PoseRegistry poseRegistry;
    private readonly InfantryPresenter presenter;

    private int idCounter;
    private readonly Dictionary<int, InfantryModel> registry = new();

    public InfantryController(HealthSystem healthSystem, FactionRegistry factionRegistry, InfantryPresenter presenter, DropSystem dropSystem, MotionSystem motionSystem, MovementSystem movementSystem, CombatSystem combatSystem, HitboxSystem hitboxSystem, EntityMapping entityMapping, PoseRegistry poseRegistry) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.presenter = presenter;
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
        UpdatePresentation();
        ClearDeadInfantry();
    }

    public int SpawnInfantry(InfantryPrototype prototype) {
        var nextId = ++idCounter;
        var model = new InfantryModel(nextId);
        registry[model.Id] = model;

        model.PoseId = poseRegistry.Add(prototype.position, default);
        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        model.MotionId = motionSystem.Add(model.PoseId, prototype.position, prototype.motionPrototype, model.HealthId);
        model.MovementId = movementSystem.Add(prototype.position, model.MotionId, model.PoseId, prototype.movementPrototype);
        model.CombatId = combatSystem.Add(prototype.combatPrototype, model.HealthId, model.MotionId, model.MovementId, model.PoseId);
        model.HitboxId = hitboxSystem.Add(model.FactionId, model.PoseId, prototype.hitboxPrototype);
        model.DropId = dropSystem.Add(prototype.dropPrototype, model.HealthId, model.PoseId);

        entityMapping.CreateMapping(model.HitboxId, new EntityComponents {
            healthId = model.HealthId,
            factionId = model.FactionId,
            motionId = model.MotionId
        });

        presenter.Add(model.Id, prototype.position, prototype.presentationPrototype);
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
        var poseState = poseRegistry.Read(model.PoseId);
        var movementState = movementSystem.ReadState(model.MovementId);
        var healthState = healthSystem.ReadState(model.HealthId);
        var motionState = motionSystem.ReadState(model.MotionId);
        var combatState = combatSystem.ReadState(model.CombatId);
        return new InfantryState (
            position: poseState.position,
            maxSpeed: movementState.maxSpeed,
            activationRadius: combatState.activationRadius,
            isAlive: !healthState.isDead,
            isGrounded: motionState.isGrounded,
            healthId: model.HealthId,
            factionIsAlie: factionRegistry.ReadState(model.FactionId).alie,
            attackActivated: combatState.attackActivated
        );
    }

    private void ClearDeadInfantry() {
        List<InfantryModel> infantryToRemove = new();

        foreach (var model in registry.Values) {
            var health = healthSystem.ReadState(model.HealthId);
            var motion = motionSystem.ReadState(model.MotionId);
            if (health.isDead && motion.isGrounded) {
                infantryToRemove.Add(model);
            }
        }

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

        presenter.Remove(model.Id);
    }

    private void UpdatePresentation() {
        foreach (var model in registry.Values) {
            var poseState = poseRegistry.Read(model.PoseId);
            var movementState = movementSystem.ReadState(model.MovementId);
            var combatState = combatSystem.ReadState(model.CombatId);
            var healthState = healthSystem.ReadState(model.HealthId);
            var motionState = motionSystem.ReadState(model.MotionId);
            presenter.Update(model.Id, poseState, movementState, combatState, healthState, motionState);
        }
    }

}
