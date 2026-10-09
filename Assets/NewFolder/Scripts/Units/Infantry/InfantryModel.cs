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
    public MotionId MotionId { get; set; }
    public PoseId PoseId { get; set; }
    public ProximityId ProximityId { get; set; }
    public RaycastId RaycastId { get; set; }
    public int AvoidanceId { get; set; }

    public Vector3 Velocity { get; set; }

    public MotionState MotionState { get; set; }
    public CombatState CombatState { get; set; }
    public PoseState PoseState { get; set; }
    
    public Vector3? MoveDestination { get; set; }
    public bool HoldMovement { get; set; }
    
    public float LastAttackTime { get; set; }
    public float AttackActivationTime { get; set; }
    public ProximityId? TargetProximityId { get; set; }

}
