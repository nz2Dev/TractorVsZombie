using Interactions;

using UnityEngine;

internal sealed class MotionModel {
    public MotionId Id { get; }
    public RagdollId BodyId { get; }
    public InfantryConfig Config { get; }

    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; } = Quaternion.identity;
    public bool Grounded { get; set; }
    public bool BecameGrounded { get; set; }
    public bool KeepAwake { get; set; }
    public bool OnTheFloor { get; set; } = true;
    public float UnsettleStartTime { get; set; } = float.NegativeInfinity;
    public float ContactWithGroundStartTime { get; set; } = float.PositiveInfinity;
    public EffectType OccurredEffectType { get; set; }
    public EffectType ActiveEffectType { get; set; }
    public Explosion ExplosionData { get; set; }

    public MotionModel(MotionId id, RagdollId bodyId, InfantryConfig config, Vector3 position) {
        Id = id;
        BodyId = bodyId;
        Config = config;
        Position = position;
    }
}
