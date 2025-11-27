
using UnityEngine;

public class ArmorModel {

    private readonly ArmorConfig config;

    public ArmorModel(int id, Vector3 position, ArmorConfig config) {
        Id = id;
        Position = position;
        this.config = config;
    }

    public int Id { get; private set; }
    public Vector3 Position { get; set; }
    public MotorVehicleId VehicleId { get; set; }
    public int CombatId { get; set; }
    public int WeaponId { get; set; }
    public int Health { get; set; }

    public int MaxHealth => config.maxHealth;
    public MotorVehicleConfig VehicleConfig => config.vehicleConfig;
    public WeaponConfig WeaponConfig => config.weaponConfig;

}