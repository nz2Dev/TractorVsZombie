
using UnityEngine;

public class ArmorModel {

    public ArmorConfig Config { get; }

    public ArmorModel(int id, Vector3 position, ArmorConfig config) {
        Id = id;
        Position = position;
        Config = config;
    }

    public int Id { get; private set; }
    public Vector3 Position { get; set; }
    
    public int VehiclePhysicsId { get; set; }
    public VehicleState VehiclePhysicsState { get; set; }
    public RewardPrototype RewardPrototype { get; set; }
    
    public float Gas { get; set; }
    public Vector3 SteerDirection { get; set; }
    public float Brakes { get; set; }

    public int CombatId { get; set; }
    public int WeaponId { get; set; }
    public bool Destroyed { get; set; }
    public int RamId { get; set; }

    public Vector3 WeaponPlacementOffset { get; set; }

}