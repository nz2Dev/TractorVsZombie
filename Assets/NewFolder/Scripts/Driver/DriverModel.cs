using UnityEngine;

public class DriverModel {

    private readonly DriverConfig config;

    public Vector3 Position { get; set; }
    public int CombatId { get; set; }
    public int VehicleId { get; set; }

    public DriverModel(DriverConfig config) {
        this.config = config;
    }

    public VehicleConfig VehicleConfig => config.vehicleConfig;
}