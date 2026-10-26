
using UnityEngine;

public class InfantryModel {

    public InfantryModel(int id) {
        Id = id;
    }

    public int Id { get; }
    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public MotionId MotionId { get; set; }
    public MovementId MovementId { get; set; }
    public CombatId CombatId { get; set; }
    public PoseId PoseId { get; set; }
    public HitboxId HitboxId { get; set; }
    public DropId DropId { get; set; }
    public MotionState MotionState { get; set; }
    public MovementState MovementState { get; set; }
    public HealthState HealthState { get; set; }
    public PoseState PoseState { get; set; }
    
}
