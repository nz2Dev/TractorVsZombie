using UnityEngine;

public class TruckModel {

    public TruckConfig Config { get; }

    public Vector3 Position { get; set; }
    public int CombatId { get; set; }
    public float MotorTorque { get; set; }
    public float SteeringDegrees { get; set; }
    public int VehiclePhysicsId { get; set; }
    public VehicleState VehiclePhysicsState { get; set; }
    public int RamId { get; set; }

    public TruckModel(TruckConfig config) {
        this.Config = config;
    }

}