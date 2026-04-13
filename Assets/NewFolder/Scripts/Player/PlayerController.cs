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
    private readonly TruckController truckController;
    private readonly HeadquarterBuildingController headquarterBuildingController;

    private readonly PlayerModel model;

    public PlayerController(PlayerView view, PlayerInput input, PlayerConfig config, PhysicsService physicsService, CombatSystem combatSystem, CameraManager cameraManager,
        RewardController rewardController, WeaponController weaponController,
        PlatformController platformController, TruckController driverController,
        HeadquarterBuildingController headquarterBuildingController) {
        this.view = view;
        this.input = input;
        this.physicsService = physicsService;
        this.combatSystem = combatSystem;
        this.cameraManager = cameraManager;
        this.rewardController = rewardController;
        this.weaponController = weaponController;
        this.platformController = platformController;
        this.truckController = driverController;
        model = new PlayerModel(config);
        this.headquarterBuildingController = headquarterBuildingController;
    }

    public void Init() {     
        CreateCamera();
        SpawnHeadquearter();
        SpawnDriver(Vector3.zero);

        bool flipFlop = false;
        for (int i = 0; i < model.InitPlatformCount; i++) {
            var loadout = (flipFlop = !flipFlop) ? model.FirstLoadoutConfig : model.SecondLoadoutConfig;
            SpawnPlatform(new Vector3(0, 0, -6f + i * -6f), out var platformId);
            EquipPlatform(platformId, loadout);
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

    private void SpawnHeadquearter() {
        var headquarterPlace = BuildingPlace.ScanSceneForPlaces().First(place => place.configType == BuildingConfigType.HeadquarterBuilding);
        headquarterBuildingController.SetHeadquearter(headquarterPlace.Position, headquarterPlace.Rotation, headquarterPlace.headquarterBuildingConfig);
    }

    private void SyncPositions() {
        model.Position = truckController.ReadVehiclePosition();
    }

    private void CollectRewards() {
        var collectedRewardStates = rewardController.CollectRewards(model.Position, 3f);
        foreach (var rewardState in collectedRewardStates) {
            if (rewardState.RewardType == RewardType.Loadout) {
                SpawnPlatform(rewardState.Position, out var platformId);
                EquipPlatform(platformId, rewardState.LoadoutConfig);
                if (model.StartOrEndCouplingOrRewards)
                    CouplePlatformInFront(platformId);
                else
                    CouplePlatformToTheEnd(platformId);
            }
        }
    }

    private void ReadDrivingInput() {
        model.DrivingInput = input.ReadDrivingInput();
    }

    private void SpawnDriver(Vector3 position) {
        truckController.Spawn(position, model.DriverConfig);
    }

    private void OperateDriver() {
        truckController.Steer(model.DrivingInput.steering);
        truckController.Drive(model.DrivingInput.gas, model.DrivingInput.boost);
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

    private void EquipPlatform(int platformId, LoadoutConfig loadout) {
        platformController.SetLoadout(platformId, loadout);
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

    private void CouplePlatformToTheEnd(int platformId) {
        int targetVehiclePhysicsId;
        if (model.CoupledPlatformIds.Count > 0) {
            var lastPlatformId = model.CoupledPlatformIds[^1];
            targetVehiclePhysicsId = platformController.GetVehiclePhysicsId(lastPlatformId);
        } else {
            targetVehiclePhysicsId = truckController.ReadVehiclePhysicsId();
        }

        platformController.Connect(platformId, targetVehiclePhysicsId);
        model.CoupledPlatformIds.Add(platformId);
    }

    private void CouplePlatformInFront(int platformId) {
        if (model.CoupledPlatformIds.Count > 0) {
            var firstPlatformId = model.CoupledPlatformIds[0];
            platformController.Disconnect(firstPlatformId);

            var newPlatformVehiclePhysicsId = platformController.GetVehiclePhysicsId(platformId);
            platformController.Connect(firstPlatformId, newPlatformVehiclePhysicsId);
        }

        platformController.Connect(platformId, truckController.ReadVehiclePhysicsId());
        model.CoupledPlatformIds.Insert(0, platformId);
    }

    private void CreateCamera() {
        cameraManager.InitTopDownFollowTarget(Vector3.zero);
    }

    private void UpdateCamera() {
        cameraManager.UpdateTopDownFollowPosition(model.Position);
    }

}