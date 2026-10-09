using System;
using System.Collections.Generic;

using UnityEngine;

public class ArmorView {

    private readonly SoundManager soundManager;
    private readonly CameraManager cameraManager;

    private Dictionary<int, ArmorVisuals> visualsRegistry = new();
    private Dictionary<int, WorldSpaceUI> uiRegistry = new ();
    private Dictionary<int, int> sfxLoopRegistry = new();

    public ArmorView(SoundManager soundManager, CameraManager cameraManager) {
        this.soundManager = soundManager;
        this.cameraManager = cameraManager;
    }

    public void Show(int armorId, Vector3 position, ArmorVisuals prefab, bool alie, AudioClip engineSFX, WorldSpaceUI worldSpaceUIPrefab) {
        var visuals = GameObject.Instantiate(prefab, position, Quaternion.identity);
        visuals.SetFactionProperties(alie);
        visualsRegistry[armorId] = visuals;
        sfxLoopRegistry[armorId] = soundManager.StartLoop(position, engineSFX);

        var ui = GameObject.Instantiate(worldSpaceUIPrefab);
        ui.TargetCamera = cameraManager.GetActiveCamera();
        uiRegistry[armorId] = ui;
    }

    public void Hide(int armorId) {
        visualsRegistry.Remove(armorId, out var visuals);
        GameObject.Destroy(visuals.gameObject);
        sfxLoopRegistry.Remove(armorId, out var sfxLoopId);
        soundManager.StopLoop(sfxLoopId);
        uiRegistry.Remove(armorId, out var ui);
        GameObject.Destroy(ui.gameObject);
    }

    public void UpdatePose(int armorId, VehicleState vehicleState) {
        var visuals = visualsRegistry[armorId];
        visuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        visuals.SetFrontAxis(vehicleState.frontAxis);
        visuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void UpdateHealthBar(int armorId, Vector3 position, int health, int maxHealth) {
        var ui = uiRegistry[armorId];
        ui.transform.position = position;

        var healthBar = ui.GetComponentInChildren<HealthBarVisuals>();
        healthBar.SetBar(health, maxHealth);
    }

    public void ShowTakeHit(int armorId) {
        var visuals = visualsRegistry[armorId];
        visuals.TriggerHitFlash();
    }

    public void UpdateSound(int armorId, float gasThrottle) {
        var visuals = visualsRegistry[armorId];
        var sfxLoopId = sfxLoopRegistry[armorId];

        var enginePitch = 0.5f + gasThrottle;
        var engineVolume = 0.5f + gasThrottle;
        soundManager.UpdateLoop(sfxLoopId, visuals.transform.position, enginePitch, engineVolume);
    }
}
