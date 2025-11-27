using System;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController {

    private readonly PlayerView view;
    private readonly CombatService combatService;
    private readonly CameraManager cameraManager;
    private readonly RewardController rewardController;
    private readonly WeaponController weaponController;
    private readonly MotorVehicleController motorVehicleController;
    private readonly TowableVehicleController towableVehicleController;
    private readonly PlatformController platformController;
    private readonly DriverController driverController;

    private readonly PlayerModel model;

    public PlayerController(PlayerView view, PlayerConfig config, CombatService combatService, CameraManager cameraManager, 
        RewardController rewardController, WeaponController weaponController, 
        MotorVehicleController vehicleController, TowableVehicleController towableVehicleController,
        PlatformController platformController, DriverController driverController) {
        this.view = view;
        this.combatService = combatService;
        this.cameraManager = cameraManager;
        this.rewardController = rewardController;
        this.weaponController = weaponController;
        this.motorVehicleController = vehicleController;
        this.towableVehicleController = towableVehicleController;
        this.platformController = platformController;
        this.driverController = driverController;
        model = new PlayerModel(config);
    }

    public void Init() {     
        CreateCamera();
        SpawnDriver(Vector3.zero);

        bool flipFlop = false;
        for (int i = 0; i < model.InitPlatformCount; i++) {
            flipFlop = !flipFlop;
            var weaponConfig = flipFlop ? model.FirstWeaponConfig : model.SecondWeaponConfig;
            AddPlatformToTheEnd(new Vector3(0, 0, -2f + i * -2f), model.DefaultPlatformConfig, weaponConfig);
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
                var groundedPosition = rewardState.Position;
                groundedPosition.y = 0;
                PickUpPlatform(groundedPosition, model.DefaultPlatformConfig, rewardState.WeaponConfig);
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

    private void AddPlatformToTheEnd(Vector3 position, PlatformConfig config, WeaponConfig weaponConfig) {
        var platformId = platformController.SpawnPlatform(position, config);
        platformController.SetWeapon(platformId, weaponConfig);
        var newPlatformState = platformController.ReadPlatformState(platformId);

        int lastVehiclePhysicsId;
        if (model.AttachedPlatformIds.Count != 0) {
            var lastAttachedPlatformState = platformController.ReadPlatformState(model.AttachedPlatformIds[^1]);
            var lastAttachedTowableVehiclePhysicsId = towableVehicleController.ReadVehiclePhysicsId(lastAttachedPlatformState.vehicleId);
            lastVehiclePhysicsId = lastAttachedTowableVehiclePhysicsId;
        } else {
            var driverMotorVehicleId = driverController.ReadVehicleId();
            lastVehiclePhysicsId = motorVehicleController.ReadVehiclePhysicsId(driverMotorVehicleId);
        }

        towableVehicleController.ConnectVehicle(newPlatformState.vehicleId, lastVehiclePhysicsId);
        model.AttachedPlatformIds.Add(platformId);
    }

    private void PickUpPlatform(Vector3 position, PlatformConfig config, WeaponConfig weaponConfig) {
        var platformId = platformController.SpawnPlatform(position, config);
        platformController.SetWeapon(platformId, weaponConfig);
        var newPlatformState = platformController.ReadPlatformState(platformId);

        if (model.AttachedPlatformIds.Count > 0) {
            var firstAttachedPlatformState = platformController.ReadPlatformState(model.AttachedPlatformIds[0]);
            var previouslyAttachedTowableVehicleId = firstAttachedPlatformState.vehicleId;
            towableVehicleController.DisconnectVehicle(previouslyAttachedTowableVehicleId);

            var newPlatformVehiclePhysicsId = towableVehicleController.ReadVehiclePhysicsId(newPlatformState.vehicleId);
            towableVehicleController.ConnectVehicle(previouslyAttachedTowableVehicleId, newPlatformVehiclePhysicsId);
        }

        var headVehicleId = driverController.ReadVehicleId();
        var headVehiclePhysicsId = motorVehicleController.ReadVehiclePhysicsId(headVehicleId);
        towableVehicleController.ConnectVehicle(newPlatformState.vehicleId, headVehiclePhysicsId);
        model.AttachedPlatformIds.Insert(0, platformId);
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