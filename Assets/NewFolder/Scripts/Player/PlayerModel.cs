using System.Collections.Generic;

using UnityEngine;

public class PlayerModel {
    
    private readonly PlayerConfig config;

    public Vector3 Position;
    public int DriverCombatId;
    public int DriverVehicleId;
    public List<int> AttachedPlatformIds { get; private set; } = new ();

    public PlayerModel(PlayerConfig config) {
        this.config = config;
    }

    public VehicleConfig DriverVehicleConfig => config.driverVehicleConfig;
    public int MaxPlatformsCount => config.maxPlatformCount;
    public PlatformConfig DefaultPlatformConfig => config.platformConfig;
    public WeaponConfig FirstWeaponConfig => config.firstWeaponConfig;
    public WeaponConfig SecondWeaponConfig => config.secondWeaponConfig;

}