using Combat;

using UnityEngine;

public class TruckModel {

    public TruckModel(TruckConfig config, Vector3 position) {
        Config = config;
        Position = position;
    }

    public TruckConfig Config { get; }

    public CombatId CombatId { get; set; }
    public int VehiclePhysicsId { get; set; }
    public int RamId { get; set; }
    
    public Vector3 Position { get; set; }
    public float Gas { get; set; }
    public float Brakes { get; set; }
    public float Steer { get; set; }
    public VehicleState VehiclePhysicsState { get; set; }

}