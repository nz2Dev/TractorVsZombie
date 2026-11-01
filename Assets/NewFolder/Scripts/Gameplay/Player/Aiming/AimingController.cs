using System.Linq;

using UnityEngine;
using UnityEngine.Playables;

public class AimingController {

    private readonly HitboxSystem hitboxSystem;
    private readonly CollisionService collisionService;
    private readonly PlatformController platformController;
    private readonly TruckController truckController;
    private readonly AssemblingController assemblingController;
    private readonly SelectingController selectingController;
    private readonly WeaponController weaponController;

    private readonly AimingView view;
    private readonly AimingModel model;

    public AimingController(AimingView view, PlatformController platformController, WeaponController weaponController, HitboxSystem hitboxSystem,
        CollisionService collisionService, AssemblingController assemblingController, SelectingController selectingController, TruckController truckController) {
        this.view = view;
        this.platformController = platformController;
        this.weaponController = weaponController;
        this.hitboxSystem = hitboxSystem;
        this.collisionService = collisionService;
        this.assemblingController = assemblingController;
        this.selectingController = selectingController;
        this.truckController = truckController;
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
        OperateTruck();
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
                OperateFromInput(platformState.loadoutState.weaponId);
            } else {
                OperateAutomatically(platformState.loadoutState.weaponId, platformState.loadoutState.weaponState, platformState.loadoutState.factionState);
            }
        }
    }

    private void OperateTruck() {
        var truckState = truckController.ReadState();
        OperateAutomatically(truckState.attachmentState.weaponId, truckState.attachmentState.weaponState, truckState.factionState);
    }

    private void OperateFromInput(int weaponId) {
        weaponController.AimWeapon(weaponId, model.AimInput.position + Vector3.up * model.AimInput.height);
    }

    private void OperateAutomatically(int weaponId, WeaponState weaponState, FactionState factionState) {
        var searchRadius = weaponState.aimConfig.range; // ?? where does radius filter should happen? here or in proximity service?
        var searchFaction = !factionState.alie;
        if (hitboxSystem.QueryNearest(weaponState.position, searchFaction, out var nearestHitboxId)) {
            var nearestPosition = hitboxSystem.GetPosition(nearestHitboxId);
            weaponController.AimWeapon(weaponId, nearestPosition + 0.5f * Vector3.up);
        }
    }

}
