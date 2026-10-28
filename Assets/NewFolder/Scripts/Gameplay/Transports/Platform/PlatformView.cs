using System.Collections.Generic;

using UnityEngine;

public class PlatformView {

    private readonly Dictionary<int, PlatformVisuals> visualsRegistry = new ();

    public void Show(int id, Vector3 position, PlatformVisuals visualsPrefab, bool alie) {
        var vehicleVisuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);        
        vehicleVisuals.SetFactionProperties(alie);
        visualsRegistry[id] = vehicleVisuals;
    }

    public void SetPose(int id, VehiclePose vehicleState) {
        var vehicleVisuals = visualsRegistry[id];
        vehicleVisuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        vehicleVisuals.SetFrontAxis(vehicleState.frontAxis);
        vehicleVisuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void ShowTakeHit(int id) {
        var visuals = visualsRegistry[id];
        visuals.TriggerHitFlash();
    }

    public void Hide(int id) {
        visualsRegistry.Remove(id, out var visuals);
        GameObject.Destroy(visuals.gameObject);
    }
}
