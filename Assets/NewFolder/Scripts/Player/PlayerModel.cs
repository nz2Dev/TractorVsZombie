using System.Collections.Generic;

using UnityEngine;

public class PlayerModel {
    
    private readonly PlayerConfig config;
    public int DriverVehicleId;
    public int DriverCombatId;
    public Vector3 DriverPosition;
    public List<int> AttachedPlatforms { get; private set; } = new ();

    public PlayerModel(PlayerConfig config) {
        this.config = config;
    }

    public VehicleConfig DriverVehicleConfig => config.driverConfig;
    public PlatformConfig DefaultPlatformConfig => config.platformConfig;
    public float DriverRamRadius => config.driverRamRadius;
    
    public AudioClip[] DriverRamImpactSound => config.driverRamImpactSound;
    public WeaponConfig FirstWeaponConfig => config.firstWeaponConfig;
    public WeaponConfig SecondWeaponConfig => config.secondWeaponConfig;
    public int MaxTrailersCount => config.maxTrailersCount;

}