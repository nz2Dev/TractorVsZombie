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
        statsDisplay.AimTransform = cameraManager.GetActiveCamera().transform;
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
        statsDisplay.ForwardInput = forward;

        var outlineDisplay = outlineRegistry[platformId];
        outlineDisplay.Position = position;
    }

    // Outline

    public void UpdateAimPoint(int platformId, Vector3 aimPoint) {
        var outlineDisplay = outlineRegistry[platformId];
        outlineDisplay.AimPoint = aimPoint;
    }

    public void SetAimIcon(int platfomrId, Sprite icon) {
        var outlineDisplay = outlineRegistry[platfomrId];
        outlineDisplay.AimIcon = icon;
    }

    public void ShowNoOutline(int platformId) {
        var outlineDisplay = outlineRegistry[platformId];
        outlineDisplay.ShowNoOutline();
    }

    public void ShowSelectedOutline(int platformId) {
        var outlineDisplay = outlineRegistry[platformId];
        outlineDisplay.ShowSelectedOutline();
    }

    public void ShowActiveOutline(int platformId) {
        var outlineDisplay = outlineRegistry[platformId];
        outlineDisplay.ShowActiveOutline();
    }

    // HUD

    public void UpdateHealthBar(int platformId, HealthState healthState) {
        var statsDisplay = statsRegistry[platformId];
        statsDisplay.ShowHealthBar(healthState.health, healthState.maxHealth);
    }

    public void HideHealthBar(int platformId) {
        var statsDisplay = statsRegistry[platformId];
        statsDisplay.HideHealthBar();
    }

    public void ShowBulletIcon(int platformId, bool show) {
        var statsDisplay = statsRegistry[platformId];
        statsDisplay.BulletIconDisplayed = show;
    }

    public void SetBulletIcon(int platformId, Sprite sprite) {
        var statsDisplay = statsRegistry[platformId];
        statsDisplay.BulletIcon = sprite;
    }

    internal void UpdateDurability(int platformId, CarryingState carryingState) {
        var statsDisplay = statsRegistry[platformId];
        statsDisplay.SetDurabilityBar(carryingState.durability, carryingState.maxDurability);
    }

}
