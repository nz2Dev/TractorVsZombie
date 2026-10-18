
using UnityEngine;

public class TruckModel {

    public TruckModel(TruckConfig config, Vector3 position) {
        Config = config;
        Position = position;
    }

    public TruckConfig Config { get; }

    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public VehicleId VehicleId { get; set; }
    public int RamId { get; set; }
    public HitboxId HitboxId { get; set; }
    
    public Vector3 Position { get; set; }
    public HealthState HealthState { get; set; }

}
