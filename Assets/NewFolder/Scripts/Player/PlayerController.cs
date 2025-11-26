using System;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController {

    private readonly PlayerView view;
    private readonly CombatService combatService;
    private readonly CameraManager cameraManager;
    private readonly RewardController rewardController;
    private readonly WeaponController weaponController;
    private readonly PlatformController platformController;
    private readonly DriverController driverController;

    private readonly PlayerModel model;

    public PlayerController(PlayerView view, PlayerConfig config,
        CombatService combatService, CameraManager cameraManager, RewardController rewardController,
        WeaponController weaponController, PlatformController platformController, DriverController driverController) {
        this.view = view;
        this.combatService = combatService;
        this.cameraManager = cameraManager;
        this.rewardController = rewardController;
        this.platformController = platformController;
        this.driverController = driverController;
        this.weaponController = weaponController;
        model = new PlayerModel(config);
    }

    public void Init() {     
        CreateCamera();
        SpawnDriver(Vector3.zero);

        bool flipFlop = false;
        for (int i = 0; i < model.MaxPlatformsCount; i++) {
            flipFlop = !flipFlop;
            var weaponConfig = flipFlop ? model.FirstWeaponConfig : model.SecondWeaponConfig;
            SpawnPlatform(new Vector3(0, 0, -2f + i * -2f), model.DefaultPlatformConfig, weaponConfig);
        }
    }

    public void Update() {
        SyncPositions();
        CollectRewards();
        ReadDrivingInput();
        OperatePlatforms();
        UpdateCamera();
    }

    private void SyncPositions() {
        model.Position = driverController.ReadVehiclePosition();
    }

    private void CollectRewards() {
        var collectedRewardStates = rewardController.CollectRewards(model.Position, 0.5f);
        foreach (var rewardState in collectedRewardStates) {
            if (rewardState.RewardType == RewardType.Weapon) {
                SpawnPlatform(rewardState.Position, model.DefaultPlatformConfig, rewardState.WeaponConfig);
            }
        }
    }

    private void SpawnDriver(Vector3 position) {
        driverController.Spawn(position, model.DriverConfig);
    }

    private void ReadDrivingInput() {
        var steerInput = Input.GetAxis("Horizontal");
        var gasInput = Input.GetAxis("Vertical");
        var boost = Input.GetKey(KeyCode.Space);
        driverController.Control(steerInput, gasInput, boost);
    }

    private int SpawnPlatform(Vector3 position, PlatformConfig config, WeaponConfig weaponConfig) {
        var headVehicleId = driverController.ReadVehicleId();
        if (model.AttachedPlatformIds.Count > 0) {
            var lastPlatformState = platformController.ReadPlatformState(model.AttachedPlatformIds[^1]);
            headVehicleId = lastPlatformState.vehicleId;
        }

        var platformId = platformController.SpawnPlatform(position, config, headVehicleId);
        model.AttachedPlatformIds.Add(platformId);
        platformController.SetWeapon(platformId, weaponConfig);
        return platformId;
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

    private void CreateCamera() {
        cameraManager.InitTopDownFollowTarget(Vector3.zero);
    }

    private void UpdateCamera() {
        cameraManager.UpdateTopDownFollowPosition(model.Position);
    }

}