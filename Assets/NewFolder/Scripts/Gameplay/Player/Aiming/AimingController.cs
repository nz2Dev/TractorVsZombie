using System.Linq;

using UnityEngine;

public class AimingController {

    private readonly HitboxSystem hitboxSystem;
    private readonly CollisionService collisionService;
    private readonly PlatformController platformController;
    private readonly AssemblingController assemblingController;
    private readonly SelectingController selectingController;
    private readonly WeaponController weaponController;

    private readonly AimingView view;
    private readonly AimingModel model;

    public AimingController(AimingView view, PlatformController platformController, WeaponController weaponController, HitboxSystem hitboxSystem,
        CollisionService collisionService, AssemblingController assemblingController, SelectingController selectingController) {
        this.view = view;
        this.platformController = platformController;
        this.weaponController = weaponController;
        this.hitboxSystem = hitboxSystem;
        this.collisionService = collisionService;
        this.assemblingController = assemblingController;
        this.selectingController = selectingController;
        model = new AimingModel();
    }

    public void Init(AimingPrototype prototype) {
        view.SetAimVisuals(prototype.aimVisualsPrefab);
    }

    private void OnManualPlatformListChanged() {
        view.HideAim();
        if (selectingController.SelectedPlatformCount != 0) {
            view.ShowAim(model.AimInput);
        }
    }

    public void Update() {
        AssemblingController.ComputeIdDiff(model.ManualPlatformIds, selectingController.SelectedPlatformIds,
            out var addedIds, out var removedIds);
        if (addedIds.Count > 0 || removedIds.Count > 0) {
            model.ManualPlatformIds.Clear();
            model.ManualPlatformIds.AddRange(selectingController.SelectedPlatformIds);
            OnManualPlatformListChanged();
        }
        ComputeAimInput();
        OperatePlatforms();
    }

    private void ComputeAimInput() {
        if (selectingController.SelectedPlatformCount == 0) {
            return;
        }

        var mousePosition = Input.mousePosition;
        var mouseRay = view.GetCameraRay(mousePosition);
        var mouseHitPoint = collisionService.GetGroundHitPosition(mouseRay);
        model.AimInput = new TopDownAimInput {
            position = mouseHitPoint,
            direction = (mouseHitPoint - assemblingController.HeadPosition).normalized,
            height = 1
        };
        view.UpdateAim(model.AimInput);
    }

    private void OperatePlatforms() {
        foreach (var platformId in assemblingController.ControlledPlatformIds) {
            var platformState = platformController.ReadPlatformState(platformId);
            if (!platformState.hasLoadout)
                continue;

            if (selectingController.SelectedPlatformIds.Contains(platformId)) {
                OperateFromInput(platformState);
            } else {
                OperateAutomatically(platformState);
            }
        }
    }

    private void OperateFromInput(PlatformState platformState) {
        weaponController.AimWeapon(platformState.loadoutState.weaponId, model.AimInput.position + Vector3.up * model.AimInput.height);
    }

    private void OperateAutomatically(PlatformState platformState) {
        var searchRadius = platformState.loadoutState.weaponState.aimConfig.range; // ?? where does radius filter should happen? here or in proximity service?
        var searchFaction = !platformState.loadoutState.factionState.alie;
        if (hitboxSystem.QueryNearest(platformState.position, searchFaction, out var nearestHitboxId)) {
            var nearestPosition = hitboxSystem.GetPosition(nearestHitboxId);
            weaponController.AimWeapon(platformState.loadoutState.weaponId, nearestPosition + 0.5f * Vector3.up);
        }
    }

}
