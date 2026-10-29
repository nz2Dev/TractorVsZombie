using System;
using System.Collections.Generic;

using UnityEngine;

public class PlatformUI {

    private readonly CameraManager cameraManager;
    private readonly Dictionary<int, PlatformUIElements> registry = new();

    public PlatformUI(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void Add(int platformId, Vector3 position, PlatformUIElements uiPrefab) {
        var ui = GameObject.Instantiate(uiPrefab, position, Quaternion.identity);
        ui.Holder.TargetCamera = cameraManager.GetActiveCamera();
        registry[platformId] = ui;
    }

    public void Remove(int platformId) {
        registry.Remove(platformId, out var ui);
        GameObject.Destroy(ui.gameObject);
    }

    public void UpdatePosition(int platformId, Vector3 position) {
        registry[platformId].UpdatePosition(position);
    }

    public void UpdateHealthBar(int platformId, HealthState healthState) {
        registry[platformId].SetHealthBar(healthState.health, healthState.maxHealth);
    }

    public void HideHealthBar(int platformId) {
        registry[platformId].HideHealthBar();
    }

    internal void UpdateDurability(int platformId, CarryingState carryingState) {
        registry[platformId].SetDurabilityBar(carryingState.durability, carryingState.maxDurability);
    }
}
