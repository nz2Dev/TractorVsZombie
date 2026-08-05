using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class PlayerController {

    private readonly PlayerView view;
    private readonly RewardController rewardController;
    private readonly TruckController truckController;

    private PlayerModel model;
    private readonly DrivingController drivingController;
    private readonly AssemblingController assemblingController;
    private readonly SelectingController selectingController;
    private readonly AimingController aimingController;

    public PlayerController(PlayerView view, PhysicsService physicsService, CombatSystem combatSystem, CameraProvider cameraProvider,
        RewardController rewardController, WeaponController weaponController, PlatformController platformController, TruckController truckController) {
        this.view = view;
        this.rewardController = rewardController;
        this.truckController = truckController;

        drivingController = new DrivingController(truckController);
        assemblingController = new AssemblingController(platformController, truckController);
        selectingController = new SelectingController(new SelectingView(view.uiDocument), platformController);
        aimingController = new AimingController(new AimingView(), cameraProvider, physicsService, combatSystem, platformController, weaponController);
        
        selectingController.OnSelectedPlatformChanged += 
            () => aimingController.SetManualPlatformIds(selectingController.SelectedPlatformIds);
    }

    public void Setup(PlayerPrototype prototype) {     
        model = new PlayerModel(prototype.config);
        aimingController.Init(prototype.aimingPrototype);
        assemblingController.Init(prototype.assemblingPrototype);
        foreach (var item in assemblingController.ControlledPlatformIds) {
            selectingController.AddOption(item);
            aimingController.AddControlledPlatformId(item);
        }
    }

    public void Update() {
        if (model == null)
            return;
        
        SyncPositions();
        CollectRewards();
        
        drivingController.Update();
        selectingController.Update();
        aimingController.SetAimSourcePosition(truckController.ReadVehiclePosition());
        aimingController.Update();
    }

    private void CollectRewards() {
        var collectedRewardStates = rewardController.CollectRewards(model.Position, 3f);
        foreach (var rewardState in collectedRewardStates) {
            if (rewardState.payload.type == RewardType.Loadout) {
                assemblingController.AddLoadout(rewardState.position, 
                    rewardState.payload.loadoutPrototype, model.Config.startOrEndCouplingOfRewards, out var platformState);
                selectingController.AddOption(platformState.platformId);
                aimingController.AddControlledPlatformId(platformState.platformId);
            }
        }
    }

    private void SyncPositions() {
        model.Position = truckController.ReadVehiclePosition();
        view.UpdateFollowCamera(model.Position);
    }

}