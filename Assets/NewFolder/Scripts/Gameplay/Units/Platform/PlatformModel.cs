
using UnityEngine;

public class PlatformModel {

    public PlatformModel(int id, Vector3 loadoutOffset) {
        Id = id;
        LoadoutOffset = loadoutOffset;
    }

    public int Id { get; }
    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public PoseId PoseId { get; set; }
    public VehicleId VehicleId { get; set; }
    public HitboxId HitboxId { get; set; }
    public int LoadoutId { get; set; }
    
    public Vector3 LoadoutOffset { get; }
    public HealthState HealthState { get; set; }

}
