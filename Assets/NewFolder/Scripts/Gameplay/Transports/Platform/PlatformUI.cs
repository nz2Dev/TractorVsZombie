using System;
using System.Collections.Generic;

using UnityEngine;

public class PlatformUI {

    private readonly CameraManager cameraManager;
    private readonly Dictionary<int, PlatformStatsDisplay> statsRegistry = new();
    private readonly Dictionary<int, PlatformOutlineDisplay> outlineRegistry = new();

    public PlatformUI(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void Add(int platformId, Vector3 position, 
        PlatformStatsDisplay statsDisplayPrefab, PlatformOutlineDisplay outlineDisplayPrefab) {
        var statsDisplay = GameObject.Instantiate(statsDisplayPrefab, position, Quaternion.identity);
        statsDisplay.CanvasRig.AimTransform = cameraManager.GetActiveCamera().transform;
        statsRegistry[platformId] = statsDisplay;

        var outlineDisplay = GameObject.Instantiate(outlineDisplayPrefab, position, Quaternion.identity);
        outlineRegistry[platformId] = outlineDisplay;
    }

    public void Remove(int platformId) {
        statsRegistry.Remove(platformId, out var statsDisplay);
        GameObject.Destroy(statsDisplay.gameObject);
        outlineRegistry.Remove(platformId, out var outlineDisplay);
        GameObject.Destroy(outlineDisplay.gameObject);
    }

    public void UpdatePose(int platformId, Vector3 position, Vector3 forward) {
        var statsDisplay = statsRegistry[platformId];
        statsDisplay.WorldPosition = position;
        statsDisplay.CanvasRig.ForwardInput = forward;

        var outlineDisplay = outlineRegistry[platformId];
        outlineDisplay.Position = position;
    }

    public void UpdateWeaponState(int platformId, WeaponState weaponState) {
        var outlineDisplay = outlineRegistry[platformId];
        outlineDisplay.Rig.AimPoint = weaponState.aimPoint;
        outlineDisplay.SetOutlineVisibility(true);
    }

    internal void HideWeaponState(int platformId) {
        var outlineDisplay = outlineRegistry[platformId];
        outlineDisplay.SetOutlineVisibility(false);
    }

    public void UpdateHealthBar(int platformId, HealthState healthState) {
        var statsDisplay = statsRegistry[platformId];
        statsDisplay.SetHealthBar(healthState.health, healthState.maxHealth);
    }

    public void HideHealthBar(int platformId) {
        var statsDisplay = statsRegistry[platformId];
        statsDisplay.HideHealthBar();
    }

    public void SetSelected(int platformId, bool selected) {
        var statsDisplay = statsRegistry[platformId];
        statsDisplay.SetSelected(selected);

        var outlineDisplay = outlineRegistry[platformId];
        outlineDisplay.SetSelected(selected);
    }

    internal void UpdateDurability(int platformId, CarryingState carryingState) {
        var statsDisplay = statsRegistry[platformId];
        statsDisplay.SetDurabilityBar(carryingState.durability, carryingState.maxDurability);
    }

}
