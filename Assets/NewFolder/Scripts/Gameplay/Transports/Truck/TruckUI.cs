using System.Collections.Generic;

using UnityEngine;

public class TruckUI {
    private readonly CameraManager cameraManager;
    private readonly Dictionary<int, TruckStatsDisplay> registry = new();

    public TruckUI(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void Add(int truckId, Vector3 position, TruckStatsDisplay displayPrefab) {
        var display = GameObject.Instantiate(displayPrefab, position, Quaternion.identity);
        display.CanvasRig.AimTransform = cameraManager.GetActiveCamera().transform;
        registry[truckId] = display;
    }

    public void Remove(int truckId) {
        registry.Remove(truckId, out var ui);
        GameObject.Destroy(ui.gameObject);
    }

    public void UpdatePose(int truckId, Vector3 position, Vector3 forward) {
        var display = registry[truckId];
        display.WorldPosition = position;
        display.CanvasRig.ForwardInput = forward;
    }

    public void UpdateHealthBar(int truckId, int health, int maxHealth) {
        var display = registry[truckId];
        display.SetHealthStat(health, maxHealth);
    }

    public void SetSelected(int truckId, bool selected) {
        if (registry.TryGetValue(truckId, out var display))
            display.SetSelected(selected);
    }
}
