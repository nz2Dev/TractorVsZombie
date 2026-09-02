using System.Collections.Generic;

using Combat;

using UnityEngine;

public class AimingController {

    private readonly ProximityService proximityService;
    private readonly CollisionService collisionService;
    private readonly PlatformController platformController;
    private readonly WeaponController weaponController;

    private readonly AimingView view;
    private readonly AimingModel model;

    public AimingController(AimingView view, PlatformController platformController, WeaponController weaponController, ProximityService proximityService, CollisionService collisionService) {
        this.view = view;
        this.platformController = platformController;
        this.weaponController = weaponController;
        this.proximityService = proximityService;
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
            if (model.ManualPlatformIds.Contains(platformId)) {
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
        var searchRadius = platformState.weaponState.aimConfig.range; // ?? where does radius filter should happen? here or in proximity service?
        var searchFaction = !platformState.combatState.alie;
        var serachProximityLayer = CombatSystem.GetProximityLayerForFaction(searchFaction);
        
        if (proximityService.QueryNearestPoint(platformState.position, serachProximityLayer, out var nearestProximityId)) {
            var nearestPosition = proximityService.GetPoint(nearestProximityId);
            weaponController.AimWeapon(platformState.weaponId, nearestPosition + 0.5f * Vector3.up);
        }
    }

}
