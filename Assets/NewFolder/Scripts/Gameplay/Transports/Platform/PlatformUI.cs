using System.Collections.Generic;

using UnityEngine;

public class PlatformUI {

    private readonly CameraManager cameraManager;
    private readonly Dictionary<int, WorldSpaceUI> registry = new();

    public PlatformUI(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void Add(int platformId, Vector3 position, WorldSpaceUI uiPrefab) {
        var ui = GameObject.Instantiate(uiPrefab, position, Quaternion.identity);
        ui.TargetCamera = cameraManager.GetActiveCamera();
        registry[platformId] = ui;
        GetHealthBar(ui).gameObject.SetActive(false);
    }

    public void Remove(int platformId) {
        registry.Remove(platformId, out var ui);
        GameObject.Destroy(ui.gameObject);
    }

    public void UpdateHealthBar(int platformId, Vector3 position, HealthState healthState) {
        var ui = registry[platformId];
        ui.transform.position = position;
        var healthBar = GetHealthBar(ui);
        healthBar.gameObject.SetActive(true);
        healthBar.SetBar(healthState.health, healthState.maxHealth);
    }

    public void HideHealthBar(int platformId) {
        var ui = registry[platformId];
        GetHealthBar(ui).gameObject.SetActive(false);    
    }

    private static HealthBarVisuals GetHealthBar(WorldSpaceUI ui) => 
        ui.GetComponentInChildren<HealthBarVisuals>(true);
}
