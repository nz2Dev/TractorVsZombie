using UnityEngine;

public class InfantryModel {
    
    public InfantryConfig Config { get; }

    public int Id { get; private set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public int CombatId { get; set; }
    public int BodyPhysicsId { get; set; }

    public bool Grounded { get; set; }
    public bool IsPhysicsOnlyMovement { get; set; }
    public Vector3 DrivenVelocity { get; set; }
    public bool IsDead { get; set; }
    public float LastAttackTime { get; set; }

    public InfantryModel(int id, InfantryConfig config) {
        Id = id;
        this.Config = config;
    }

}