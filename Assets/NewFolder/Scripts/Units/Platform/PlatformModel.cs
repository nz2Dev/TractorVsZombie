using Combat;

using UnityEngine;

public class PlatformModel {

    public PlatformModel(int id, Vector3 position, PlatformConfig config, Vector3 loadoutOffset) {
        Id = id;
        Position = position;
        Config = config;
        LoadoutOffset = loadoutOffset;
    }

    public int Id { get; }
    public PlatformConfig Config { get; }
    public Vector3 LoadoutOffset { get; }

    public CombatId CombatId { get; set; }
    public int VehiclePhysicsId { get; set; }
    public ProximityId ProximityId { get; set; }
    public RaycastId RaycastId { get; set; }

    public int LoadoutId { get; set; }
    public int RamId { get; set; }

    public Vector3 Position { get; set; }
    public VehicleState VehiclePhysicsState { get; set; }

}
