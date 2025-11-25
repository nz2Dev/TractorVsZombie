using System.Collections.Generic;

using UnityEngine;

public class PlayerModel {
    
    private readonly PlayerConfig config;

    public List<int> AttachedPlatformIds { get; private set; } = new ();

    public PlayerModel(PlayerConfig config) {
        this.config = config;
    }

    public DriverConfig DriverConfig => config.driverConfig;
    public int MaxPlatformsCount => config.maxPlatformCount;
    public PlatformConfig DefaultPlatformConfig => config.platformConfig;
    public WeaponConfig FirstWeaponConfig => config.firstWeaponConfig;
    public WeaponConfig SecondWeaponConfig => config.secondWeaponConfig;

}