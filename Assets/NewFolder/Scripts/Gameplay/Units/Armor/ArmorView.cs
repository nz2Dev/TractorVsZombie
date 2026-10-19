using System.Collections.Generic;

using UnityEngine;

public class ArmorView {

    private readonly Dictionary<int, ArmorVisuals> registry = new();

    public void Show(int armorId, Vector3 position, bool alie, ArmorVisuals visualsPrefab) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
        visuals.SetFactionProperties(alie);
        registry[armorId] = visuals;
    }

    public void Hide(int armorId) {
        registry.Remove(armorId, out var visuals);
        GameObject.Destroy(visuals.gameObject);
    }

    public void SetPose(int armorId, VehicleState vehicleState) {
        var visuals = registry[armorId];
        visuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        visuals.SetFrontAxis(vehicleState.frontAxis);
        visuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void ShowTakeHit(int armorId) {
        var visuals = registry[armorId];
        visuals.TriggerHitFlash();
    }
}
