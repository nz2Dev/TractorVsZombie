
using UnityEngine;

public class PlatformModel {

    public PlatformModel(int id, Vector3 position, Vector3 loadoutOffset) {
        Id = id;
        Position = position;
        LoadoutOffset = loadoutOffset;
    }

    public int Id { get; }
    public Vector3 LoadoutOffset { get; }

    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public VehicleId VehicleId { get; set; }
    public HitboxId HitboxId { get; set; }

    public int LoadoutId { get; set; }
    public int RamId { get; set; }

    public Vector3 Position { get; set; }
    public HealthState HealthState { get; set; }

}
