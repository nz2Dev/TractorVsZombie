using System.Collections.Generic;

using UnityEngine;

public class PlatformView {

    private readonly CameraManager cameraManager;

    private readonly Dictionary<int, PlatformVisuals> visualsRegistry = new ();
    private readonly Dictionary<int, WorldSpaceUI> uiRegistry = new ();

    public PlatformView(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void AddPlatform(int id, Vector3 position, PlatformVisuals visualsPrefab, bool alie, WorldSpaceUI worldSpaceUIPrefab) {
        var vehicleVisuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);        
        vehicleVisuals.SetFactionProperties(alie);
        visualsRegistry[id] = vehicleVisuals;
        var ui = GameObject.Instantiate(worldSpaceUIPrefab);
        ui.TargetCamera = cameraManager.GetActiveCamera();
        uiRegistry[id] = ui;
    }

    public void UpdatePlatformPose(int id, VehicleState vehicleState) {
        var vehicleVisuals = visualsRegistry[id];
        vehicleVisuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        vehicleVisuals.SetFrontAxis(vehicleState.frontAxis);
        vehicleVisuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void UpdateHealthBar(int id, Vector3 position, int health, int maxHealth) {
        var ui = uiRegistry[id];
        ui.transform.position = position;
        var healthBar = ui.GetComponentInChildren<HealthBarVisuals>();
        healthBar.SetBar(health, maxHealth);
    }

    public void ShowTakeHit(int id) {
        var visuals = visualsRegistry[id];
        visuals.TriggerHitFlash();
    }

    public void RemovePlatform(int id) {
        var visuals = visualsRegistry[id];
        GameObject.Destroy(visuals.gameObject);
        visualsRegistry.Remove(id);

        var ui = uiRegistry[id];
        GameObject.Destroy(ui.gameObject);
        uiRegistry.Remove(id);
    }
}