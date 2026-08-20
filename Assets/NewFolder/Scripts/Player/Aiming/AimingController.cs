using System;
using System.Collections.Generic;

using Compatibility;

using UnityEngine;

public class AimingController {

    private readonly CameraProvider cameraProvider;

    private readonly RaycastService raycastService;
    private readonly CombatSystem combatSystem;

    private readonly PlatformController platformController;
    private readonly WeaponController weaponController;

    private readonly AimingView view;
    private readonly AimingModel model;

    public AimingController(AimingView view, CameraProvider cameraProvider, RaycastService raycastService, CombatSystem combatSystem, PlatformController platformController, WeaponController weaponController) {
        this.view = view;
        this.cameraProvider = cameraProvider;
        this.raycastService = raycastService;
        this.combatSystem = combatSystem;
        this.platformController = platformController;
        this.weaponController = weaponController;
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
        // if GetScreePointRay is moved to the View, there is no need in CameraProvider at all
        var mouseRay = cameraProvider.GetScreenPointRay(mousePosition);
        var mouseHitPoint = raycastService.GetGroundHitPosition(mouseRay);
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
        var searchRadius = platformState.weaponState.aimConfig.range;
        if (combatSystem.GetClosestEnemyAgentInRange(platformState.combatId, searchRadius, out var agentInfo)) {
            weaponController.AimWeapon(platformState.weaponId, agentInfo.position + 0.5f * agentInfo.height * Vector3.up);
        }
    }

}
