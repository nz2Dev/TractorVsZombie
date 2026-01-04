using UnityEngine;

public class PlatformModel {
    
    private readonly PlatformConfig config;

    public PlatformModel(int id, Vector3 position, PlatformConfig config) {
        this.Id = id;
        this.config = config;
        Position = position;
    }

    public int Id { get; private set; }
    public Vector3 Position { get; set; }
    public TowableVehicleId VehicleId { get; set; }
    public int CombatId { get; set; }
    public int WeaponId { get; set; }
    public int RamId { get; set; }
    public WeaponConfig WeaponConfig { get; set; }
    public TowableVehicleConfig VehicleConfig => config.vehicleConfig;
    public RamConfig RamConfig => config.ramConfig;

}
