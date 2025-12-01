using System.Collections.Generic;

using UnityEngine;

public struct DrivingInput {
    public float gas;
    public float steering;
    public bool boost;
}

public class PlayerModel {
    
    private readonly PlayerConfig config;

    public Vector3 Position { get; set; }
    public DrivingInput DrivingInput { get; set; }
    public int SelectedPlatformId { get; set; }
    public List<int> ControlledPlatformIds { get; private set; } = new ();

    public PlayerModel(PlayerConfig config) {
        this.config = config;
    }

    public DriverConfig DriverConfig => config.driverConfig;
    public int InitPlatformCount => config.initPlatformCount;
    public PlatformConfig DefaultPlatformConfig => config.platformConfig;
    public WeaponConfig FirstWeaponConfig => config.firstWeaponConfig;
    public WeaponConfig SecondWeaponConfig => config.secondWeaponConfig;

}