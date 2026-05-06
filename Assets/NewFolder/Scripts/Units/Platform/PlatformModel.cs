using UnityEngine;

public class PlatformModel {
    
    public int Id { get; }
    public PlatformConfig Config { get; }

    public PlatformModel(int id, Vector3 position, PlatformConfig config) {
        Id = id;
        Position = position;
        Config = config;
    }

    public Vector3 Position { get; set; }
    public int VehiclePhysicsId { get; set; }
    public VehicleState VehiclePhysicsState { get; set; }
    public int CombatId { get; set; }
    public int LoadoutId { get; set; }
    public int RamId { get; set; }
    
    public Vector3 LoadoutOffset { get; set; }

}
