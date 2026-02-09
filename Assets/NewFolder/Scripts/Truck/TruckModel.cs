using UnityEngine;

public class TruckModel {

    private readonly TruckConfig config;

    public Vector3 Position { get; set; }
    public int CombatId { get; set; }
    public MotorVehicleId VehicleId { get; set; }
    public int RamId { get; set; }

    public TruckModel(TruckConfig config) {
        this.config = config;
    }

    public MotorVehicleConfig VehicleConfig => config.vehicleConfig;
    public RamConfig RamConfig => config.ramConfig;
}