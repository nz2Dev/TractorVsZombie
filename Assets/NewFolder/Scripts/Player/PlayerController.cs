using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class PlayerController {

    private readonly PlayerView view;
    private readonly PlayerInput input;
    private readonly CombatSystem combatSystem;
    private readonly PhysicsService physicsService;
    private readonly CameraProvider cameraProvider;
    private readonly RewardController rewardController;
    private readonly WeaponController weaponController;
    private readonly PlatformController platformController;
    private readonly TruckController truckController;

    private PlayerModel model;
    private DrivingController drivingController;
    private AssemblingController assemblingController;
    private SelectingController selectingController;

    public PlayerController(PlayerView view, PlayerInput input, PhysicsService physicsService, CombatSystem combatSystem, CameraProvider cameraProvider,
        RewardController rewardController, WeaponController weaponController, PlatformController platformController, TruckController truckController) {
        this.view = view;
        this.input = input;
        this.physicsService = physicsService;
        this.combatSystem = combatSystem;
        this.cameraProvider = cameraProvider;
        this.rewardController = rewardController;
        this.weaponController = weaponController;
        this.platformController = platformController;
        this.truckController = truckController;

        drivingController = new DrivingController(truckController);
        assemblingController = new AssemblingController(platformController, truckController);
        selectingController = new SelectingController(new SelectingView(view.uiDocument), platformController);
        selectingController.OnSelectedPlatformChanged += OnSelectedPlatformChanged;
    }

    public void Setup(PlayerPrototype prototype) {     
        model = new PlayerModel(prototype.config);
        view.SetAimVisuals(prototype.aimVisualsPrefab);
        
        assemblingController.Init(prototype.assemblingPrototype);
        foreach (var item in assemblingController.ControlledPlatformIds) {
            selectingController.AddOption(item);
        }
    }

    public void Update() {
        if (model == null)
            return;
            
        SyncPositions();
        CollectRewards();
        
        drivingController.Update();
        selectingController.Update();

        ComputeAimInput();
        OperatePlatforms();
    }

    private void CollectRewards() {
        var collectedRewardStates = rewardController.CollectRewards(model.Position, 3f);
        foreach (var rewardState in collectedRewardStates) {
            if (rewardState.payload.type == RewardType.Loadout) {
                assemblingController.AddLoadout(rewardState.position, 
                    rewardState.payload.loadoutPrototype, model.Config.startOrEndCouplingOfRewards, out var platformState);
                selectingController.AddOption(platformState.platformId);
            }
        }
    }

    private void SyncPositions() {
        model.Position = truckController.ReadVehiclePosition();
        view.UpdateFollowCamera(model.Position);
    }

    private void OnSelectedPlatformChanged() {
        view.HideAim();
        if (selectingController.SelectedPlatformCount != 0) {
            view.ShowAim(model.AimInput);
        }
    }

    private void ComputeAimInput() {
        if (selectingController.SelectedPlatformCount == 0) {
            return;
        }
        
        var mousePosition = input.ReadMousePosition();
        var mouseRay = cameraProvider.GetScreenPointRay(mousePosition);
        var mouseHitPoint = physicsService.GetGroundHitPosition(mouseRay);
        model.AimInput = new TopDownAimInput {
            position = mouseHitPoint,
            direction = (mouseHitPoint - model.Position).normalized,
            height = 1
        };
        view.UpdateAim(model.AimInput);
    }

    private void OperatePlatforms() {
        foreach (var platformId in assemblingController.ControlledPlatformIds) {
            if (selectingController.IsSelected(platformId)) {
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
        var searchRadius = platformState.weaponState.aimConfig.range;
        if (combatSystem.GetClosestEnemyAgentInRange(platformState.combatId, searchRadius, out var agentInfo)) {
            weaponController.AimWeapon(platformState.weaponId, agentInfo.position + 0.5f * agentInfo.height * Vector3.up);
        }
    }

}