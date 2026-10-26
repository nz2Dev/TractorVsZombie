
using UnityEngine;

public enum ChargeAttackEvents {
    None,
    ChargeActivated,
    AttackActivated,
    Canceled
}

public class InfantryModel {

    public InfantryModel(int id, InfantryConfig config) {
        Id = id;
        Config = config;
    }

    public int Id { get; }
    public InfantryConfig Config { get; }
    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public MotionId MotionId { get; set; }
    public MovementId MovementId { get; set; }
    public PoseId PoseId { get; set; }
    public HitboxId HitboxId { get; set; }
    public DropId DropId { get; set; }
    public MotionState MotionState { get; set; }
    public MovementState MovementState { get; set; }
    public HealthState HealthState { get; set; }
    public PoseState PoseState { get; set; }
    
    public float LastAttackTime { get; set; }
    public float AttackActivationTime { get; set; }
    public HitboxId? TargetHitboxId { get; set; }

    public ChargeAttackEvents AttackEvents { get; set; } = ChargeAttackEvents.None;

}
