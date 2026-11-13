using System.Collections.Generic;

using UnityEngine;

public class TruckUI {
    private readonly CameraManager cameraManager;
    private readonly Dictionary<int, TransportStatsUI> registry = new();

    public TruckUI(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void Add(int truckId, Vector3 position, TransportStatsUI uiPrefab) {
        var ui = GameObject.Instantiate(uiPrefab, position, Quaternion.identity);
        ui.TargetCamera = cameraManager.GetActiveCamera();
        registry[truckId] = ui;
    }

    public void Remove(int truckId) {
        registry.Remove(truckId, out var ui);
        GameObject.Destroy(ui.gameObject);
    }

    public void UpdatePose(int truckId, Vector3 position, Vector3 forward) {
        var ui = registry[truckId];
        ui.WorldPosition = position;
        ui.UnitForward = forward;
    }

    public void UpdateHealthBar(int truckId, int health, int maxHealth) {
        var ui = registry[truckId];
        ui.SetHealthStat(health, maxHealth);
    }
}
