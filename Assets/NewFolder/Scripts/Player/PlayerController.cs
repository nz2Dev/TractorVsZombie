using System;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController {

    private readonly PlayerView view;
    private readonly WeaponController weaponController;
    private readonly VehicleController vehicleController;
    private readonly PlatformController platformController;

    private readonly CombatService combatService;
    private readonly CameraManager cameraManager;
    private readonly SoundManager soundManager;

    private readonly PlayerModel model;

    public PlayerController(PlayerView view, CombatService combatService, CameraManager cameraManager,
        SoundManager soundManager, PlayerConfig config, VehicleController vehicleController, PlatformController platformController, WeaponController weaponController) {
        this.combatService = combatService;
        this.view = view;
        this.combatService = combatService;
        this.cameraManager = cameraManager;
        this.soundManager = soundManager;

        this.vehicleController = vehicleController;
        model = new PlayerModel(config);
        this.platformController = platformController;
        this.weaponController = weaponController;
    }

    public void Init() {
        cameraManager.InitTopDownFollowTarget(Vector3.zero);
        
        SpawnDriverVehicle(Vector3.zero);

        for (int i = 0; i < model.MaxTrailersCount; i++) {
            SpawnPlatform(new Vector3(0, 0, -2f + i * -2f), model.DefaultPlatformConfig);
        }

        bool flipFlop = false;
        foreach (var platformId in model.AttachedPlatformIds) {
            flipFlop = !flipFlop;
            var weaponConfig = flipFlop ? model.FirstWeaponConfig : model.SecondWeaponConfig;
            EquipePlatform(platformId, weaponConfig);
        }
    }

    public void Update() {
        SyncDriveVehiclePositions();
        DriveHeadVehicle();
        UpdateDriverRamCombat();
        OperatePlatforms();
        UpdateCamera();
    }

    public void ExtendConvoy(Vector3 position, WeaponConfig equipment) {
        var platformId = SpawnPlatform(position, model.DefaultPlatformConfig);
        EquipePlatform(platformId, equipment);
    }

    public Vector3 GetPlayerPosition() {
        return model.DriverPosition;
    }

    private void UpdateCamera() {
        cameraManager.UpdateTopDownFollowPosition(model.DriverPosition);
    }

    private void SpawnDriverVehicle(Vector3 driveVehiclePosition) {
        model.DriverVehicleId = vehicleController.SpawnVehicle(driveVehiclePosition, model.DriverVehicleConfig);
        model.DriverCombatId = combatService.RegisterAgent(driveVehiclePosition, alie: true);
    }

    private void SyncDriveVehiclePositions() {
        model.DriverPosition = vehicleController.GetVehiclePosition(model.DriverVehicleId);
        combatService.UpdateAgentPosition(model.DriverCombatId, model.DriverPosition);
    }

    private void DriveHeadVehicle() {
        var steerInput = Input.GetAxis("Horizontal");
        vehicleController.SteerVehicle(model.DriverVehicleId, steerInput);

        var gasInput = Input.GetAxis("Vertical");
        var boost = Input.GetKey(KeyCode.Space);
        vehicleController.DriveVehicle(model.DriverVehicleId, gasInput, boost);
    }

    private void UpdateDriverRamCombat() {
        var affectedCount = combatService.ApplyExplosionDamage(model.DriverCombatId, model.DriverPosition, model.DriverRamRadius, damage: 0);
        for (int i = 0; i < affectedCount; i++) {
            var position = model.DriverPosition + UnityEngine.Random.onUnitSphere * model.DriverRamRadius;
            soundManager.PlayEffectDelayed(position, i * 0.05f, model.DriverRamImpactSound);
        }
    }

    private int SpawnPlatform(Vector3 position, PlatformConfig config) {
        var headVehicleId = model.DriverVehicleId;
        if (model.AttachedPlatformIds.Count > 0) {
            var lastPlatformState = platformController.ReadPlatformState(model.AttachedPlatformIds[^1]);
            headVehicleId = lastPlatformState.vehicleId;
        }

        var platformId = platformController.SpawnPlatform(position, config, headVehicleId);
        model.AttachedPlatformIds.Add(platformId);
        return platformId;
    }

    private void EquipePlatform(int platformId, WeaponConfig weaponConfig) {
        platformController.SetWeapon(platformId, weaponConfig);
    }

    private void OperatePlatforms() {
        foreach (var platformId in model.AttachedPlatformIds) {
            var platformState = platformController.ReadPlatformState(platformId);
            var searchRadius = platformState.weaponConfig.aimConfig.range;
            
            if (combatService.GetClosestEnemyAgentInRange(platformState.combatId, searchRadius, out var agentInfo)) {
                weaponController.AimWeapon(platformState.weaponId, agentInfo.position + 0.5f * agentInfo.height * Vector3.up);
            }
        }
    }

}