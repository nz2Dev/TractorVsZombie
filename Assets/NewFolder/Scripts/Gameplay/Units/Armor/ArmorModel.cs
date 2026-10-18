

using UnityEngine;

public class ArmorModel {

    public ArmorModel(int id, Vector3 position, ArmorConfig config, Vector3 weaponPlacementOffset) {
        Id = id;
        Position = position;
        Config = config;
        WeaponPlacementOffset = weaponPlacementOffset;
    }

    public int Id { get; }
    public ArmorConfig Config { get; }    
    public Vector3 WeaponPlacementOffset { get; }
    
    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public VehicleId VehicleId { get; set; }
    public PoseId PoseId { get; set; }
    public HitboxId HitboxId { get; set; }
    public DropId DropId { get; set; }

    public int WeaponId { get; set; }
    public int RamId { get; set; }

    public Vector3 Position { get; set; }
    public HealthState HealthState { get; set; }
}
