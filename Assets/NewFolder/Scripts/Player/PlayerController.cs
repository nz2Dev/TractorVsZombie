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
    private readonly CouplingController couplingController;

    private readonly PlayerModel model;

    public PlayerController(PlayerView view, PlayerConfig config, CombatService combatService, CameraManager cameraManager, 
        RewardController rewardController, WeaponController weaponController,
        PlatformController platformController, DriverController driverController,
        CouplingController couplingController) {
        this.view = view;
        this.combatService = combatService;
        this.cameraManager = cameraManager;
        this.rewardController = rewardController;
        this.weaponController = weaponController;
        this.platformController = platformController;
        this.driverController = driverController;
        this.couplingController = couplingController;
        model = new PlayerModel(config);
    }

    public void Init() {     
        CreateCamera();
        SpawnDriver(Vector3.zero);
        CreateCoupling();

        bool flipFlop = false;
        for (int i = 0; i < model.InitPlatformCount; i++) {
            var weaponConfig = (flipFlop = !flipFlop) ? model.FirstWeaponConfig : model.SecondWeaponConfig;
            SpawnPlatform(new Vector3(0, 0, -2f + i * -2f), out var platformId);
            EquipePlatform(platformId, weaponConfig);
            CouplePlatformToTheEnd(platformId);
        }
    }

    public void Update() {
        SyncPositions();
        CollectRewards();
        ReadDrivingInput();
        OperateDriver();
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
                SpawnPlatform(rewardState.Position, out var platformId);
                EquipePlatform(platformId, rewardState.WeaponConfig);
                CouplePlatformInFront(platformId);
            }
        }
    }

    private void ReadDrivingInput() {
        model.DrivingInput = new DrivingInput {
            gas = Input.GetAxis("Vertical"),
            steering = Input.GetAxis("Horizontal"),
            boost = Input.GetKey(KeyCode.Space),
        };
    }

    private void SpawnDriver(Vector3 position) {
        driverController.Spawn(position, model.DriverConfig);
    }

    private void OperateDriver() {
        driverController.Control(
            model.DrivingInput.steering, 
            model.DrivingInput.gas, 
            model.DrivingInput.boost
        );
    }

    private void SpawnPlatform(Vector3 position, out int platformId) {
        platformId = platformController.SpawnPlatform(position, model.DefaultPlatformConfig);
        model.ControlledPlatformIds.Add(platformId);
        view.AddPlatform(platformController.ReadPlatformState(platformId));
    }

    private void EquipePlatform(int platformId, WeaponConfig weaponConfig) {
        platformController.SetWeapon(platformId, weaponConfig);
        view.UpdatePlatform(platformController.ReadPlatformState(platformId));
    }

    private void OperatePlatforms() {
        foreach (var platformId in model.ControlledPlatformIds) {
            var platformState = platformController.ReadPlatformState(platformId);
            var searchRadius = platformState.weaponConfig.aimConfig.range;
            
            if (combatService.GetClosestEnemyAgentInRange(platformState.combatId, searchRadius, out var agentInfo)) {
                weaponController.AimWeapon(platformState.weaponId, agentInfo.position + 0.5f * agentInfo.height * Vector3.up);
            }
        }
    }

    private void CreateCoupling() {
        couplingController.Create(driverController.ReadVehicleId());
    }

    private void CouplePlatformToTheEnd(int platformId) {
        var platformState = platformController.ReadPlatformState(platformId);
        couplingController.AddTowable(platformState.vehicleId);
    }

    private void CouplePlatformInFront(int platformId) {
        var platformState = platformController.ReadPlatformState(platformId);
        couplingController.InsertTowable(platformState.vehicleId);
    }

    private void CreateCamera() {
        cameraManager.InitTopDownFollowTarget(Vector3.zero);
    }

    private void UpdateCamera() {
        cameraManager.UpdateTopDownFollowPosition(model.Position);
    }

}