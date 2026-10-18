using System.Collections.Generic;

using UnityEngine;

public class ArmorUI {
    
    private readonly CameraManager cameraManager;
    private Dictionary<int, WorldSpaceUI> uiRegistry = new ();

    public ArmorUI(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    public void Add(int armorId, Vector3 position, ArmorUIPrototype uiPrototype) {
        var ui = GameObject.Instantiate(uiPrototype.worldSpaceUIPrefab, position, Quaternion.identity);
        ui.TargetCamera = cameraManager.GetActiveCamera();
        uiRegistry[armorId] = ui;
    }

    public void Remove(int armorId) {
        uiRegistry.Remove(armorId, out var ui);
        GameObject.Destroy(ui.gameObject);
    }

    public void UpdateHealthBar(int armorId, Vector3 position, int health, int maxHealth) {
        var ui = uiRegistry[armorId];
        ui.transform.position = position;

        var healthBar = ui.GetComponentInChildren<HealthBarVisuals>();
        healthBar.SetBar(health, maxHealth);
    }
}