using System.Collections.Generic;

using UnityEngine;

public class ArmorUI {

    private readonly CameraManager cameraManager;
    private readonly Dictionary<int, ArmorStatsDisplay> registry = new(); 

    public ArmorUI(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void Add(int armorId, Vector3 position, ArmorStatsDisplay displayPrefab) {
        var display = GameObject.Instantiate(displayPrefab, position, Quaternion.identity);
        display.CanvasRig.AimTransform = cameraManager.GetActiveCamera().transform;
        registry[armorId] = display;
    }

    public void Remove(int armorId) {
        registry.Remove(armorId, out var display);
        GameObject.Destroy(display.gameObject);
    }

    public void UpdatePose(int armorId, Vector3 position, Vector3 forward) {
        var display = registry[armorId];
        display.WorldPosition = position;
        display.CanvasRig.ForwardInput = forward;
    }

    public void UpdateHealthState(int armorId, int health, int maxHealth) {
        var display = registry[armorId];
        display.SetHealthState(health, maxHealth);
    }
}
