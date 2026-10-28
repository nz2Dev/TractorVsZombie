using System.Collections.Generic;

using UnityEngine;

public sealed class CombatSystem {
    
    private readonly HealthSystem healthSystem;
    private readonly EntityMapping entityMapping;
    private readonly PoseRegistry poseRegistry;
    private readonly MotionSystem motionSystem;
    private readonly HitboxSystem hitboxSystem;
    private readonly MovementSystem movementSystem;

    private int idCounter;
    private readonly Dictionary<CombatId, CombatModel> registry = new();

    public CombatSystem(HealthSystem healthSystem, EntityMapping entityMapping, PoseRegistry poseRegistry, MotionSystem motionSystem, HitboxSystem hitboxSystem, MovementSystem movementSystem) {
        this.healthSystem = healthSystem;
        this.entityMapping = entityMapping;
        this.poseRegistry = poseRegistry;
        this.motionSystem = motionSystem;
        this.hitboxSystem = hitboxSystem;
        this.movementSystem = movementSystem;
    }

    public CombatId Add(CombatPrototype prototype, HealthId healthId, MotionId motionId, MovementId movementId, PoseId poseId) {
        var id = new CombatId(++idCounter);
        registry[id] = new CombatModel(id, prototype, healthId, motionId, movementId, poseId);
        return id;
    }

    public void Remove(CombatId id) {
        registry.Remove(id);
    }

    public void SetTarget(CombatId id, HitboxId targetHitboxId) {
        registry[id].TargetHitboxId = targetHitboxId;
    }

    public void ClearTarget(CombatId id) {
        registry[id].TargetHitboxId = null;
    }

    public CombatState ReadState(CombatId id) {
        var model = registry[id];
        return new CombatState {
            attackEvent = model.AttackEvent,
            attackActivated = model.AttackActivationTime > model.LastAttackTime,
            attackDuration = model.Config.attackDuration,
            activationRadius = model.Config.activationRadius
        };
    }

    public void Update() {
        foreach (var model in registry.Values) {
            model.AttackEvent = CombatEvent.None;
            if (healthSystem.ReadState(model.HealthId).isDead)
                continue;

            var motionState = motionSystem.ReadState(model.MotionId);
            var attackCanceled = !model.TargetHitboxId.HasValue || !motionState.isGrounded;
            if (attackCanceled) {
                model.AttackActivationTime = model.LastAttackTime - 1;
                model.AttackEvent = CombatEvent.Canceled;
                continue;
            }

            var targetHitboxId = model.TargetHitboxId.Value;
            var lostAbilityForTarget = !entityMapping.TryFindByHitboxId(targetHitboxId, out var targetComponents)
                || !targetComponents.healthId.HasValue;
            if (lostAbilityForTarget) {
                model.TargetHitboxId = null;
                model.AttackActivationTime = model.LastAttackTime - 1;
                model.AttackEvent = CombatEvent.Canceled;
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
                model.AttackEvent = CombatEvent.ChargeActivated;
            }

            var charging = Time.time > model.AttackActivationTime && Time.time < model.AttackActivationTime + model.Config.attackDuration;
            if (charging) {
                movementSystem.HoldMovement(model.MovementId);
            }

            var notActivated = model.AttackActivationTime < model.LastAttackTime;
            var notChargedYet = Time.time < model.AttackActivationTime + model.Config.attackDuration;
            if (notActivated || notChargedYet)
                continue;

            model.LastAttackTime = Time.time;
            model.AttackEvent = CombatEvent.AttackActivated;

            var targetHitboxState = hitboxSystem.ReadState(targetHitboxId);
            var outOfReach = Vector3.Distance(pose.position, targetPosition) > model.Config.activationRadius + targetHitboxState.radius;
            if (outOfReach)
                continue;

            healthSystem.DealDamage(targetComponents.healthId.Value, new DamageInput {
                damageSource = pose.position,
                damageType = DamageType.Punch,
                damage = model.Config.damage
            });
        }
    }
}
