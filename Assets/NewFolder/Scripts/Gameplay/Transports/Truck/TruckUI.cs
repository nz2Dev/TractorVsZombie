using System.Collections.Generic;

using UnityEngine;

public class TruckUI {
    
    private readonly CameraManager cameraManager;
    private TruckStatsDisplay display;

    public TruckUI(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void Add(Vector3 position, TruckStatsDisplay displayPrefab) {
        display = GameObject.Instantiate(displayPrefab, position, Quaternion.identity);
        display.CanvasRig.AimTransform = cameraManager.GetActiveCamera().transform;
    }

    public void Remove() {
        GameObject.Destroy(display.gameObject);
        display = null;
    }

    public void UpdatePose(Vector3 position, Vector3 forward) {
        display.WorldPosition = position;
        display.CanvasRig.ForwardInput = forward;
    }

    public void UpdateHealthBar(int health, int maxHealth) {
        display.SetHealthStat(health, maxHealth);
    }

    public void SetSelected(bool selected) {
        display.SetSelected(selected);
    }

}
