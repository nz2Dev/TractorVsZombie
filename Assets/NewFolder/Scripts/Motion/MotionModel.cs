using Combat;

using Interactions;

using UnityEngine;

internal sealed class MotionModel {
    
    public MotionId Id { get; }
    public PoseId HostPoseId { get; }
    public CombatId HostCombatId { get; }
    public RagdollId BodyId { get; }
    public InfantryConfig Config { get; }

    public bool Grounded { get; set; }
    public bool OnTheFloor { get; set; } = true;
    public float UnsettleStartTime { get; set; } = float.NegativeInfinity;
    public float ContactWithGroundStartTime { get; set; } = float.PositiveInfinity;
    public EffectType OccurredEffectType { get; set; }
    public EffectType ActiveEffectType { get; set; }
    public Explosion ExplosionData { get; set; }

    public MotionModel(MotionId id, PoseId poseId, RagdollId bodyId, InfantryConfig config, CombatId hostCombatId) {
        Id = id;
        HostPoseId = poseId;
        BodyId = bodyId;
        Config = config;
        HostCombatId = hostCombatId;
    }
}
