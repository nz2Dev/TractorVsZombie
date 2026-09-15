using System;

using UnityEngine;

public class HeadquarterBuildingView {
    
    private readonly CameraManager cameraManager;

    private HeadquarterBuildingVisuals visuals;
    private HealthBarVisuals healthBarVisuals;

    public HeadquarterBuildingView(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void ShowHeadquarter(Vector3 position, Quaternion rotation, HeadquarterBuildingVisuals visualsPrefab, WorldSpaceUI worldSpaceUIPrefab) {
        visuals = GameObject.Instantiate(visualsPrefab, position, rotation);
        var worldSpaceUI = GameObject.Instantiate(worldSpaceUIPrefab, position, Quaternion.identity);
        worldSpaceUI.TargetCamera = cameraManager.GetActiveCamera();
        healthBarVisuals = worldSpaceUI.GetComponentInChildren<HealthBarVisuals>();
    }

    public void ShowTakeHit() {
        visuals.TriggerHitFlash();
    }

    internal void ShowHeadquarterDestoryed() {
        GameObject.Destroy(visuals);
        GameObject.Destroy(healthBarVisuals.GetComponentInParent<WorldSpaceUI>().gameObject);
    }

    internal void UpdateHealth(int health, int maxHealth) {
        healthBarVisuals.SetBar(health, maxHealth);
    }
}