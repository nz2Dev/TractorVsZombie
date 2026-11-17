using System;
using System.Collections.Generic;

using UnityEngine;

public class PlatformUI {

    private readonly CameraManager cameraManager;
    private readonly Dictionary<int, PlatformStatsDisplay> registry = new();

    public PlatformUI(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void Add(int platformId, Vector3 position, PlatformStatsDisplay uiPrefab) {
        var display = GameObject.Instantiate(uiPrefab, position, Quaternion.identity);
        display.CanvasRig.AimTransform = cameraManager.GetActiveCamera().transform;
        registry[platformId] = display;
    }

    public void Remove(int platformId) {
        registry.Remove(platformId, out var display);
        GameObject.Destroy(display.gameObject);
    }

    public void UpdatePose(int platformId, Vector3 position, Vector3 forward) {
        var display = registry[platformId];
        display.WorldPosition = position;
        display.CanvasRig.ForwardInput = forward;
    }

    public void UpdateHealthBar(int platformId, HealthState healthState) {
        registry[platformId].SetHealthBar(healthState.health, healthState.maxHealth);
    }

    public void HideHealthBar(int platformId) {
        registry[platformId].HideHealthBar();
    }

    public void SetSelected(int platformId, bool selected) {
        if (registry.TryGetValue(platformId, out var display))
            display.SetSelected(selected);
    }

    internal void UpdateDurability(int platformId, CarryingState carryingState) {
        registry[platformId].SetDurabilityBar(carryingState.durability, carryingState.maxDurability);
    }
}
