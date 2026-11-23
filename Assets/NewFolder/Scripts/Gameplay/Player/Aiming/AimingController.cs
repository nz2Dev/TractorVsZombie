using System.Collections.Generic;
using System.Linq;

using UnityEngine;

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

    public void Update() {
        ComputeAimInput();
        view.SetAimVisible(selectingController.HasSelection);
        OperateUnits();
    }

    private void ComputeAimInput() {
        if (!selectingController.HasSelection) {
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

    private void OperateUnits() {
        foreach (var option in selectingController.Options)
            Operate(option);
    }

    private void Operate(SelectionOption option) {
        int weaponId;
        WeaponState weaponState;
        FactionState factionState;

        if (option.entityType == SelectionEntityType.Platform) {
            var platformState = platformController.ReadPlatformState(option.entityId);
            if (!platformState.hasLoadout)
                return;
            weaponId = platformState.loadoutState.weaponId;
            weaponState = platformState.loadoutState.weaponState;
            factionState = platformState.loadoutState.factionState;
        } else {
            var truckState = truckController.ReadState();
            weaponId = truckState.attachmentState.weaponId;
            weaponState = truckState.attachmentState.weaponState;
            factionState = truckState.factionState;
        }

        if (selectingController.SelectedOptions.Contains(option))
            OperateFromInput(weaponId);
        else
            OperateAutomatically(weaponId, weaponState, factionState);
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
