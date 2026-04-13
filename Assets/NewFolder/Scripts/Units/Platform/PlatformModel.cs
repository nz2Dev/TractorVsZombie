using UnityEngine;

public class PlatformModel {
    
    public PlatformConfig Config { get; }

    public PlatformModel(int id, Vector3 position, PlatformConfig config) {
        Id = id;
        Config = config;
        Position = position;
    }

    public int Id { get; private set; }
    public Vector3 Position { get; set; }
    public int VehiclePhysicsId { get; set; }
    public VehicleState VehiclePhysicsState { get; set; }
    public int CombatId { get; set; }
    public int WeaponId { get; set; }
    public WeaponConfig WeaponConfig { get; set; }
    public Vector3 WeaponPlacementOffset { get; set; }
    public int RamId { get; set; }

}
