using System.Collections.Generic;

using PlasticPipe.PlasticProtocol.Messages;

using UnityEngine;

public class TruckUI {
    
    private readonly CameraManager cameraManager;
    private TruckStatsDisplay statsDisplay;
    private TruckOutlineDisplay outlineDisplay;

    public TruckUI(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void Add(Vector3 position, TruckStatsDisplay displayPrefab, TruckOutlineDisplay outlineDisplayPrefab) {
        statsDisplay = GameObject.Instantiate(displayPrefab, position, Quaternion.identity);
        statsDisplay.AimTransform = cameraManager.GetActiveCamera().transform;
        outlineDisplay = GameObject.Instantiate(outlineDisplayPrefab, position, Quaternion.identity);
    }

    public void Remove() {
        GameObject.Destroy(statsDisplay.gameObject);
        statsDisplay = null;
        GameObject.Destroy(outlineDisplay.gameObject);
        outlineDisplay = null;
    }

    // TruckHUD.UpdatePose(position, forward) makes more sense
    public void UpdatePose(Vector3 position, Vector3 forward) {
        statsDisplay.WorldPosition = position;
        statsDisplay.ForwardInput = forward;

        outlineDisplay.Position = position;
    }

    // Outline

    public void UpdateAimPoint(Vector3 aimPoint) {
        outlineDisplay.AimPoint = aimPoint;
    }

    public void SetAimIcon(Sprite icon) {
        outlineDisplay.AimIcon = icon;
    }

    public void ShowNoOutline() {
        outlineDisplay.ShowNoOutline();
    }

    public void ShowSelectedOutline() {
        outlineDisplay.ShowSelectedOutline();
    }

    public void ShowActiveOutline() {
        outlineDisplay.ShowActiveOutline();
    }

    // HUD

    public void UpdateHealthBar(int health, int maxHealth) {
        statsDisplay.ShowHealthBar(health, maxHealth);
    }

    public void HideHealthBar() {
        statsDisplay.HideHealthBar();
    }

    public void ShowBulletIcon(bool show) {
        statsDisplay.BulletIconDisplayed = show;
    }

    public void SetBulletIcon(Sprite sprite) {
        statsDisplay.BulletIcon = sprite;
    }
}
