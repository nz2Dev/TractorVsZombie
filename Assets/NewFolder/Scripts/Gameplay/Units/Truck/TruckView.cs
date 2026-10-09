using System;

using UnityEngine;

public class TruckView {

    private readonly SoundManager soundManager;
    private readonly CameraManager cameraManager;
    
    private WorldSpaceUI ui;
    private TruckVisuals visuals;
    private int sfxLoopId;

    public TruckView(SoundManager soundManager, CameraManager cameraManager) {
        this.soundManager = soundManager;
        this.cameraManager = cameraManager;
    }

    public void Show(Vector3 position, TruckVisuals prefab, bool alie, AudioClip engineSFX, WorldSpaceUI worldSpaceUIPrefab) {
        visuals = GameObject.Instantiate(prefab, position, Quaternion.identity);
        sfxLoopId = soundManager.StartLoop(position, engineSFX);
        visuals.SetFactionProperties(alie);

        ui = GameObject.Instantiate(worldSpaceUIPrefab);
        ui.TargetCamera = cameraManager.GetActiveCamera();
    }

    public void UpdatePose(VehicleState vehicleState) {
        visuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        visuals.SetFrontAxis(vehicleState.frontAxis);
        visuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void UpdateHealthBar(Vector3 position, int health, int maxHealth) {
        ui.transform.position = position;
        var healthBar = ui.GetComponentInChildren<HealthBarVisuals>();
        healthBar.SetBar(health, maxHealth);
    }

    public void ShowTakeHit() {
        visuals.TriggerHitFlash();
    }

    internal void UpdateSound(float gasThrottle) {
        var enginePitch = 0.5f + gasThrottle;
        var engineVolume = 0.1f + gasThrottle;
        soundManager.UpdateLoop(sfxLoopId, visuals.transform.position, enginePitch, engineVolume);
    }

    internal void Remove() {
        GameObject.Destroy(visuals.gameObject);
        soundManager.StopLoop(sfxLoopId);
        GameObject.Destroy(ui.gameObject);
    }

}