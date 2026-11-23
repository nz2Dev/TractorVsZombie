using System.Collections.Generic;

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
        statsDisplay.CanvasRig.AimTransform = cameraManager.GetActiveCamera().transform;
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
        statsDisplay.CanvasRig.ForwardInput = forward;

        outlineDisplay.Position = position;
    }

    public void UpdateAimPoint(Vector3 aimPoint) {
        outlineDisplay.AimPoint = aimPoint;
    }

    public void ShowAim(bool show) {
        outlineDisplay.AimVisibility = show;
    }

    public void UpdateHealthBar(int health, int maxHealth) {
        statsDisplay.SetHealthStat(health, maxHealth);
    }

    public void SetSelected(bool selected) {
        statsDisplay.SetSelected(selected);
        outlineDisplay.Selection = selected;
    }

}
