using Combat;

using UnityEngine;

public class InfantryModel {
    public InfantryModel(int id, InfantryConfig config, float maxSpeed, RewardPrototype rewardPrototype) {
        Id = id;
        Config = config;
        MaxSpeed = maxSpeed;
        RewardPrototype = rewardPrototype;
    }

    public int Id { get; }
    public InfantryConfig Config { get; }
    public float MaxSpeed { get; } // compatibimity, is obtained from avoidance config
    public RewardPrototype RewardPrototype { get; }

    public CombatId CombatId { get; set; }
    public bool CombatIsAlie { get; set; }
    public InteractionId InteractionId { get; set; }
    public RagdollId BodyPhysicsId { get; set; }
    public ProximityId ProximityId { get; set; }
    public RaycastId RaycastId { get; set; }
    public int AvoidanceId { get; set; }

    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }
    public Quaternion Rotation { get; set; }
    
    public bool IsDead { get; set; }
    public float LastAttackTime { get; set; }
    public bool IsPhysicsOnlyMovement { get; set; }

    public bool Grounded { get; set; }
    public float UnsettleStartTime { get; set; } = float.NegativeInfinity;
    public bool OnTheFloor { get; set; } = true;
    public float ContactWithGroundStartTime { get; set; } = float.PositiveInfinity;

}