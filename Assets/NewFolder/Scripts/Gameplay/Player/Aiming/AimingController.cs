using System;
using System.Collections.Generic;


using UnityEngine;

public class AimingController {

    private readonly HitboxSystem hitboxSystem;
    private readonly CollisionService collisionService;
    private readonly PlatformController platformController;
    private readonly WeaponController weaponController;

    private readonly AimingView view;
    private readonly AimingModel model;

    public AimingController(AimingView view, PlatformController platformController, WeaponController weaponController, HitboxSystem hitboxSystem, CollisionService collisionService) {
        this.view = view;
        this.platformController = platformController;
        this.weaponController = weaponController;
        this.hitboxSystem = hitboxSystem;
        this.collisionService = collisionService;
        model = new AimingModel();
    }

    public void Init(AimingPrototype prototype) {
        view.SetAimVisuals(prototype.aimVisualsPrefab);
    }

    public void SetAimSourcePosition(Vector3 position) {
        model.AimSourcePosition = position;
    }

    internal void SetManualPlatformIds(IEnumerable<int> manualPlatformIds) {
        model.ManualPlatformIds.Clear();
        model.ManualPlatformIds.AddRange(manualPlatformIds);
        OnManualPlatformListChanged();
    }

    internal void AddControlledPlatformId(int platformId) {
        model.ControlledPlatformIds.Add(platformId);
    }

    internal void RemoveControlledPlatformId(int platformId) {
        model.ControlledPlatformIds.Remove(platformId);
    }

    private void OnManualPlatformListChanged() {
        view.HideAim();
        if (model.ManualPlatformIds.Count != 0) {
            view.ShowAim(model.AimInput);
        }
    }

    public void Update() {
        ComputeAimInput();
        OperatePlatforms();
    }

    private void ComputeAimInput() {
        if (model.ManualPlatformIds.Count == 0) {
            return;
        }

        var mousePosition = Input.mousePosition;
        var mouseRay = view.GetCameraRay(mousePosition);
        var mouseHitPoint = collisionService.GetGroundHitPosition(mouseRay);
        model.AimInput = new TopDownAimInput {
            position = mouseHitPoint,
            direction = (mouseHitPoint - model.AimSourcePosition).normalized,
            height = 1
        };
        view.UpdateAim(model.AimInput);
    }

    private void OperatePlatforms() {
        foreach (var platformId in model.ControlledPlatformIds) {
            var platformState = platformController.ReadPlatformState(platformId);
            if (!platformState.hasLoadout)
                continue;

            if (model.ManualPlatformIds.Contains(platformId)) {
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
