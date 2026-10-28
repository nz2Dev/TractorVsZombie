using System.Collections.Generic;

using UnityEngine;

public class ArmorUI {

    private readonly CameraManager cameraManager;
    private readonly Dictionary<int, WorldSpaceUI> registry = new(); 

    public ArmorUI(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void Add(int armorId, Vector3 position, WorldSpaceUI uiPrefab) {
        var ui = GameObject.Instantiate(uiPrefab, position, Quaternion.identity);
        ui.TargetCamera = cameraManager.GetActiveCamera();
        registry[armorId] = ui;
    }

    public void Remove(int armorId) {
        registry.Remove(armorId, out var ui);
        GameObject.Destroy(ui.gameObject);
    }

    public void UpdateHealthBar(int armorId, Vector3 position, int health, int maxHealth) {
        var ui = registry[armorId];
        ui.transform.position = position;
        var healthBar = ui.GetComponentInChildren<HealthBarVisuals>();
        healthBar.SetBar(health, maxHealth);
    }
}
