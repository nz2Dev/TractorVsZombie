using Combat;

using UnityEngine;

public class TruckModel {

    public TruckModel(TruckConfig config, Vector3 position) {
        Config = config;
        Position = position;
    }

    public TruckConfig Config { get; }

    public CombatId CombatId { get; set; }
    public VehicleId VehicleId { get; set; }
    public int RamId { get; set; }
    public ProximityId ProximityId { get; set; }
    public RaycastId RaycastId { get; set; }
    
    public Vector3 Position { get; set; }
    public CombatState CombatState { get; set; }

}
