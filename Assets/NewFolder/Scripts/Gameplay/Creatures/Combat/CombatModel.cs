internal sealed class CombatModel {
    public CombatModel(CombatId id, CombatPrototype prototype, HealthId healthId, MotionId motionId, MovementId movementId, PoseId poseId) {
        Id = id;
        Config = prototype.config;
        HealthId = healthId;
        MotionId = motionId;
        MovementId = movementId;
        PoseId = poseId;
    }

    public CombatId Id { get; }
    public CombatConfig Config { get; }
    public HealthId HealthId { get; }
    public MotionId MotionId { get; }
    public MovementId MovementId { get; }
    public PoseId PoseId { get; }

    public float LastAttackTime { get; set; }
    public float AttackActivationTime { get; set; }
    public HitboxId? TargetHitboxId { get; set; }
    public CombatEvent AttackEvent { get; set; }
}
