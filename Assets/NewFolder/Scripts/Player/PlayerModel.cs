using System.Collections.Generic;

using UnityEngine;

public struct DrivingInput {
    public float gas;
    public float steering;
    public bool boost;
}

public struct TopDownAimInput {
    public float height;
    public Vector3 position; 
    public Vector3 direction;
}

public class PlayerModel {
    
    private readonly PlayerConfig config;

    public Vector3 Position { get; set; }
    public DrivingInput DrivingInput { get; set; }
    public List<int> SelectedPlatformIds { get; set; } = new ();
    public List<int> ControlledPlatformIds { get; private set; } = new ();
    public TopDownAimInput AimInput { get; set; }

    public PlayerModel(PlayerConfig config) {
        this.config = config;
    }

    public DriverConfig DriverConfig => config.driverConfig;
    public int InitPlatformCount => config.initPlatformCount;
    public bool StartOrEndCouplingOrRewards => config.startOrEndCouplingOfRewards;
    public PlatformConfig DefaultPlatformConfig => config.platformConfig;
    public WeaponConfig FirstWeaponConfig => config.firstWeaponConfig;
    public WeaponConfig SecondWeaponConfig => config.secondWeaponConfig;

}