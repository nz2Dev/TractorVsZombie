
using Combat;

using UnityEngine;

public class ArmorModel {

    public ArmorModel(int id, Vector3 position, ArmorConfig config, RewardPrototype rewardPrototype, Vector3 weaponPlacementOffset) {
        Id = id;
        Position = position;
        Config = config;
        RewardPrototype = rewardPrototype;
        WeaponPlacementOffset = weaponPlacementOffset;
    }

    public int Id { get; }
    public ArmorConfig Config { get; }    
    public RewardPrototype RewardPrototype { get; }
    public Vector3 WeaponPlacementOffset { get; }
    
    public CombatId CombatId { get; set; }
    public int VehiclePhysicsId { get; set; }
    public ProximityId ProximityId { get; set; }
    public RaycastId RaycastId { get; set; }

    public int WeaponId { get; set; }
    public int RamId { get; set; }

    public Vector3 Position { get; set; }
    public VehicleState VehiclePhysicsState { get; set; }
    
    public float Gas { get; set; }
    public Vector3 SteerDirection { get; set; }
    public float Brakes { get; set; }
    public bool Destroyed { get; set; }

}