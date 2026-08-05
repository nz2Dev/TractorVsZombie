using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class PlayerController {

    private readonly PlayerView view;
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
    private AimingController aimingController;

    public PlayerController(PlayerView view, PhysicsService physicsService, CombatSystem combatSystem, CameraProvider cameraProvider,
        RewardController rewardController, WeaponController weaponController, PlatformController platformController, TruckController truckController) {
        this.view = view;
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
        aimingController = new AimingController(new AimingView(), cameraProvider, physicsService, combatSystem, platformController, weaponController);
        selectingController.OnSelectedPlatformChanged += () => {
            aimingController.SetManualPlatformIds(selectingController.SelectedPlatformIds);
        };
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