using UnityEngine;

public class InfantryPresenter {

    private readonly InfantryView view;
    private readonly InfantryAudio audio;

    public InfantryPresenter(InfantryView view, InfantryAudio audio) {
        this.view = view;
        this.audio = audio;
    }

    public void Add(int infantryId, Vector3 position, InfantryPresentationPrototype prototype) {
        view.AddVisuals(infantryId, position, prototype.visualsPrefab);
    }

    public void Remove(int infantryId) {
        view.RemoveVisuals(infantryId);
    }

    public void Update(int infantryId, PoseState poseState, MovementState movementState, CombatState combatState, HealthState healthState, MotionState motionState) {
        float speedNormalized = movementState.velocity.magnitude / movementState.maxSpeed;
        view.ShowMovementSpeed(infantryId, speedNormalized);
        if (motionState.isGrounded) {
            audio.RegisterFootstep(poseState.position, speedNormalized);
        }
        
        if (!healthState.isDead || !motionState.isGrounded) {
            view.SetTargetPose(infantryId, poseState.position, poseState.rotation);
        }
        
        switch (combatState.attackEvent) {
            case CombatEvent.ChargeActivated:
                view.ShowCharge(infantryId, combatState.attackDuration);
                break;
            case CombatEvent.AttackActivated:
                view.PlayChargeAttack(infantryId);
                break;
            case CombatEvent.Canceled:
                view.ShowDischarge(infantryId);
                break;
        }

        var healthNormalized = (float) healthState.health / Mathf.Max(healthState.maxHealth, 1);
        view.SetHealthNormalized(infantryId, healthNormalized);

        if (healthState.damageResult.HasValue) {
            view.ShowTakeHit(infantryId);

            var damageResult = healthState.damageResult.Value;
            if (damageResult.damageWasFatal) {
                if (damageResult.damageType == DamageType.Projectile && motionState.isGrounded) {
                    var sourceToPosition = poseState.position - damageResult.damageSource; 
                    sourceToPosition.y = 0;
                    var outOfSourceRotation = Quaternion.LookRotation(
                        -sourceToPosition.normalized, Vector3.up);
                    view.SetCurrentPose(infantryId, poseState.position, outOfSourceRotation);
                    view.ShowThrownAway(infantryId);
                } else {
                    view.ShowDisolveDeath(infantryId);
                }
            }
        }
    }
}
