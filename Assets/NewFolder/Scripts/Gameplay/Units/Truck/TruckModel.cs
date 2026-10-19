
using UnityEngine;

public class TruckModel {

    public TruckModel(Vector3 position) {
        Position = position;
    }

    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public VehicleId VehicleId { get; set; }
    public int RamId { get; set; }
    public HitboxId HitboxId { get; set; }
    
    public Vector3 Position { get; set; }
    public HealthState HealthState { get; set; }

}
