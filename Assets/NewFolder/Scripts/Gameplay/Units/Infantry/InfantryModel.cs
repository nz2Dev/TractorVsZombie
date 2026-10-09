using Combat;

using UnityEngine;

public class InfantryModel {

    public InfantryModel(int id, InfantryConfig config, RewardPrototype rewardPrototype) {
        Id = id;
        Config = config;
        RewardPrototype = rewardPrototype;
    }

    public int Id { get; }
    public InfantryConfig Config { get; }
    public RewardPrototype RewardPrototype { get; }

    public CombatId CombatId { get; set; }
    public bool CombatIsAlie { get; set; }
    public MotionId MotionId { get; set; }
    public MovementId MovementId { get; set; }
    public PoseId PoseId { get; set; }
    public ProximityId ProximityId { get; set; }
    public RaycastId RaycastId { get; set; }
    public MotionState MotionState { get; set; }
    public MovementState MovementState { get; set; }
    public CombatState CombatState { get; set; }
    public PoseState PoseState { get; set; }
    
    public float LastAttackTime { get; set; }
    public float AttackActivationTime { get; set; }
    public ProximityId? TargetProximityId { get; set; }

}
