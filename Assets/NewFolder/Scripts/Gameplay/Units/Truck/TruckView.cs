using System.Collections.Generic;

using UnityEngine;

public class TruckView {
    private readonly Dictionary<int, TruckVisuals> registry = new();

    public void Show(int truckId, Vector3 position, bool alie, TruckVisuals visualsPrefab) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
        visuals.SetFactionProperties(alie);
        registry[truckId] = visuals;
    }

    public void Hide(int truckId) {
        registry.Remove(truckId, out var visuals);
        GameObject.Destroy(visuals.gameObject);
    }

    public void SetPose(int truckId, VehicleState vehicleState) {
        var visuals = registry[truckId];
        visuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        visuals.SetFrontAxis(vehicleState.frontAxis);
        visuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void ShowTakeHit(int truckId) {
        registry[truckId].TriggerHitFlash();
    }
}
