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
    }

    public void Setup(PlayerPrototype prototype) {     
        model = new PlayerModel(prototype.config);
        view.SetAimVisuals(prototype.aimVisualsPrefab);
        
        assemblingController.Init(prototype.assemblingPrototype);
        foreach (var item in assemblingController.ControlledPlatformIds) {
            view.AddPlatform(platformController.ReadPlatformState(item));
        }
    }

    public void Update() {
        if (model == null)
            return;
            
        SyncPositions();
        CollectRewards();
        
        drivingController.Update();

        ReadPlatformSelectionInput();
        ComputeAimInput();
        OperatePlatforms();
    }

    private void CollectRewards() {
        var collectedRewardStates = rewardController.CollectRewards(model.Position, 3f);
        foreach (var rewardState in collectedRewardStates) {
            if (rewardState.payload.type == RewardType.Loadout) {
                assemblingController.AddLoadout(rewardState.position, 
                    rewardState.payload.loadoutPrototype, model.Config.startOrEndCouplingOfRewards, out var platformState);
                view.AddPlatform(platformState);
            }
        }
    }

    private void SyncPositions() {
        model.Position = truckController.ReadVehiclePosition();
        view.UpdateFollowCamera(model.Position);
    }

    private void ReadPlatformSelectionInput() {
        var toggledIds = Enumerable.Empty<int>();

        if (input.ReadSelectAllPressed()) {
            bool partiallySelected = 
                model.SelectedPlatformIds.Count != assemblingController.ControlledPlatformIds.Count;
            
            toggledIds = partiallySelected
                ? assemblingController.ControlledPlatformIds.Except(model.SelectedPlatformIds)
                : assemblingController.ControlledPlatformIds;
        } else if (input.ReadSelectionIndexPressed(out var pressedIndex)) {
            toggledIds = new[] { assemblingController.ControlledPlatformIds[pressedIndex] };
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
        var searchRadius = platformState.weaponState.aimConfig.range;
        if (combatSystem.GetClosestEnemyAgentInRange(platformState.combatId, searchRadius, out var agentInfo)) {
            weaponController.AimWeapon(platformState.weaponId, agentInfo.position + 0.5f * agentInfo.height * Vector3.up);
        }
    }

}