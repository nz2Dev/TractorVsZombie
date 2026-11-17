using System;

using UnityEngine;

public class HeadquarterBuildingView {
    
    private readonly CameraManager cameraManager;

    private HeadquarterBuildingVisuals visuals;
    private HeadquarterStatsDisplay display;

    public HeadquarterBuildingView(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void ShowHeadquarter(Vector3 position, Quaternion rotation, HeadquarterBuildingVisuals visualsPrefab, HeadquarterStatsDisplay displayPrefab) {
        visuals = GameObject.Instantiate(visualsPrefab, position, rotation);
        display = GameObject.Instantiate(displayPrefab, position, Quaternion.identity);
        display.CanvasRig.AimTransform = cameraManager.GetActiveCamera().transform;
    }

    public void ShowTakeHit() {
        visuals.TriggerHitFlash();
    }

    internal void Remove() {
        GameObject.Destroy(visuals.gameObject);
        GameObject.Destroy(display.gameObject);
    }

    internal void UpdateHealth(int health, int maxHealth) {
        display.SetHealthState(health, maxHealth);
    }
}