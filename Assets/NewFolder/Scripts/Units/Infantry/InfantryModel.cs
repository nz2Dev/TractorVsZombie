using UnityEngine;

public class InfantryModel {
    
    public int Id { get; }
    public InfantryConfig Config { get; }

    public InfantryModel(int id, InfantryConfig config, float maxSpeed) {
        Id = id;
        this.Config = config;
        MaxSpeed = maxSpeed;
    }

    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }
    public Quaternion Rotation { get; set; }
    public int CombatId { get; set; }
    public int BodyPhysicsId { get; set; }
    public int AvoidanceId { get; set; }
    public float MaxSpeed { get; } // compatibimity, is obtained from avoidance config

    public RewardPrototype RewardPrototype { get; set; }

    public bool Grounded { get; set; }
    public bool IsPhysicsOnlyMovement { get; set; }
    public bool IsDead { get; set; }
    public float LastAttackTime { get; set; }

    

}