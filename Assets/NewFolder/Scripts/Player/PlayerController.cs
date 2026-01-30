using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class PlayerController {

    private readonly PlayerView view;
    private readonly PlayerInput input;
    private readonly CombatSystem combatSystem;
    private readonly PhysicsService physicsService;
    private readonly CameraManager cameraManager;
    private readonly RewardController rewardController;
    private readonly WeaponController weaponController;
    private readonly PlatformController platformController;
    private readonly DriverController driverController;
    private readonly CouplingController couplingController;

    private readonly PlayerModel model;

    public PlayerController(PlayerView view, PlayerInput input, PlayerConfig config, PhysicsService physicsService, CombatSystem combatSystem, CameraManager cameraManager,
        RewardController rewardController, WeaponController weaponController,
        PlatformController platformController, DriverController driverController,
        CouplingController couplingController) {
        this.view = view;
        this.input = input;
        this.physicsService = physicsService;
        this.combatSystem = combatSystem;
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
        ReadPlatformSelectionInput();
        ComputeAimInput();
        OperatePlatforms();
        UpdateCamera();
    }

    private void SyncPositions() {
        model.Position = driverController.ReadVehiclePosition();
    }

    private void CollectRewards() {
        var collectedRewardStates = rewardController.CollectRewards(model.Position, 3f);
        foreach (var rewardState in collectedRewardStates) {
            if (rewardState.RewardType == RewardType.Weapon) {
                SpawnPlatform(rewardState.Position, out var platformId);
                EquipePlatform(platformId, rewardState.WeaponConfig);
                if (model.StartOrEndCouplingOrRewards) {
                    CouplePlatformInFront(platformId);
                } else {
                    CouplePlatformToTheEnd(platformId);
                }
            }
        }
    }

    private void ReadDrivingInput() {
        model.DrivingInput = input.ReadDrivingInput();
    }

    private void SpawnDriver(Vector3 position) {
        driverController.Spawn(position, model.DriverConfig);
    }

    private void OperateDriver() {
        driverController.Control(model.DrivingInput.steering, model.DrivingInput.gas, model.DrivingInput.boost);
    }

    private void ReadPlatformSelectionInput() {
        var toggledIds = Enumerable.Empty<int>();

        if (input.ReadSelectAllPressed()) {
            bool partiallySelected = 
                model.SelectedPlatformIds.Count != model.ControlledPlatformIds.Count;
            
            toggledIds = partiallySelected
                ? model.ControlledPlatformIds.Except(model.SelectedPlatformIds)
                : model.ControlledPlatformIds;
        } else if (input.ReadSelectionIndexPressed(out var pressedIndex)) {
            toggledIds = new[] { model.ControlledPlatformIds[pressedIndex] };
        }

        bool hasEffect = false;
        foreach (var id in toggledIds) {
            hasEffect = true;
            if (!model.SelectedPlatformIds.Remove(id))
                model.SelectedPlatformIds.Add(id);
        }
        
        if (hasEffect) {
            OnSelectedPlatformChanged();
        }
    }

    private void OnSelectedPlatformChanged() {
        DeactivateSelectionControl();
        if (model.SelectedPlatformIds.Count != 0) {
            ActivateSelectionControl();
        }
    }

    private void ActivateSelectionControl() {
        view.ShowAim(model.AimInput);
        foreach (var selectedPlatformId in model.SelectedPlatformIds) {
            view.ShowPlatformSelected(platformController.ReadPlatformState(selectedPlatformId));
        }
    }

    private void DeactivateSelectionControl() {
        view.HideAim();
        view.ShowNoPlatformSelected();
    }

    private void ComputeAimInput() {
        if (model.SelectedPlatformIds.Count == 0) {
            return;
        }
        
        var mousePosition = input.ReadMousePosition();
        var mouseRay = cameraManager.GetCameraRay(mousePosition);
        var mouseHitPoint = physicsService.GetGroundHitPosition(mouseRay);
        model.AimInput = new TopDownAimInput {
            position = mouseHitPoint,
            direction = (mouseHitPoint - model.Position).normalized,
            height = 0.5f
        };
        view.UpdateAim(model.AimInput);
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
            if (model.SelectedPlatformIds.Contains(platformId)) {
                OperateFromInput(platformController.ReadPlatformState(platformId));
            } else {
                OperateAutomatically(platformController.ReadPlatformState(platformId));
            }
        }
    }

    private void OperateFromInput(PlatformState platformState) {
        weaponController.AimWeapon(platformState.weaponId, model.AimInput.position + Vector3.up * model.AimInput.height);
    }

    private void OperateAutomatically(PlatformState platformState) {
        var searchRadius = platformState.weaponConfig.aimConfig.range;
        if (combatSystem.GetClosestEnemyAgentInRange(platformState.combatId, searchRadius, out var agentInfo)) {
            weaponController.AimWeapon(platformState.weaponId, agentInfo.position + 0.5f * agentInfo.height * Vector3.up);
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