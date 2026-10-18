using System;
using System.Collections.Generic;

using UnityEngine;

public class ArmorView {

    private Dictionary<int, ArmorVisuals> visualsRegistry = new();
    
    public ArmorView() {
    }

    public void Show(int armorId, Vector3 position, bool alie, ArmorViewPrototype prototype) {
        var visuals = GameObject.Instantiate(prototype.visualsPrefab, position, Quaternion.identity);
        visuals.SetFactionProperties(alie);
        visualsRegistry[armorId] = visuals;
    }

    public void Hide(int armorId) {
        visualsRegistry.Remove(armorId, out var visuals);
        GameObject.Destroy(visuals.gameObject);
    }

    public void UpdatePose(int armorId, VehicleState vehicleState) {
        var visuals = visualsRegistry[armorId];
        visuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        visuals.SetFrontAxis(vehicleState.frontAxis);
        visuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void ShowTakeHit(int armorId) {
        var visuals = visualsRegistry[armorId];
        visuals.TriggerHitFlash();
    }
    
}
