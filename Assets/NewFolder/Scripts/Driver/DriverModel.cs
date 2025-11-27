using UnityEngine;

public class DriverModel {

    private readonly DriverConfig config;

    public Vector3 Position { get; set; }
    public int CombatId { get; set; }
    public MotorVehicleId VehicleId { get; set; }

    public DriverModel(DriverConfig config) {
        this.config = config;
    }

    public MotorVehicleConfig VehicleConfig => config.vehicleConfig;
}