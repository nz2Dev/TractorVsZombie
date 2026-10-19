using System.Collections.Generic;

using UnityEngine;

public class TruckUI {
    private readonly CameraManager cameraManager;
    private readonly Dictionary<int, WorldSpaceUI> registry = new();

    public TruckUI(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void Add(int truckId, Vector3 position, WorldSpaceUI uiPrefab) {
        var ui = GameObject.Instantiate(uiPrefab, position, Quaternion.identity);
        ui.TargetCamera = cameraManager.GetActiveCamera();
        registry[truckId] = ui;
    }

    public void Remove(int truckId) {
        registry.Remove(truckId, out var ui);
        GameObject.Destroy(ui.gameObject);
    }

    public void UpdateHealthBar(int truckId, Vector3 position, int health, int maxHealth) {
        var ui = registry[truckId];
        ui.transform.position = position;
        ui.GetComponentInChildren<HealthBarVisuals>().SetBar(health, maxHealth);
    }
}
