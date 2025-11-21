using System.Collections.Generic;

using UnityEngine;

public class HostVehicle {
    public Vector3 Position { get; set; }
    public int VehicleId { get; set; }
    public int CombatId { get; set; }
    public int WeaponId { get; set; }
}

public class PlayerModel {
    
    private readonly PlayerConfig config;
    public int DriverVehicleId;
    public int DriverCombatId;
    public Vector3 DriverPosition;
    public List<HostVehicle> HostVehicles { get; private set; } = new ();

    public PlayerModel(PlayerConfig config) {
        this.config = config;
    }

    public VehicleConfig DriverVehicleConfig => config.driverConfig;
    public float DriverRamRadius => config.driverRamRadius;
    public float DriverRewardCollectRadius => config.driverRewardCollectRadius;
    public AudioClip[] DriverRamImpactSound => config.driverRamImpactSound;
    public VehicleConfig TrailerVehicleConfig => config.trailerConfig;
    public WeaponConfig FirstWeaponConfig => config.firstWeaponConfig;
    public WeaponConfig SecondWeaponConfig => config.secondWeaponConfig;
    public int MaxTrailersCount => config.maxTrailersCount;

}