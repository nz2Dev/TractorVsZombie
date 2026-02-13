using System.Collections.Generic;

using UnityEngine;

public class PlatformView {
    private readonly Dictionary<int, PlatformVisuals> visualsRegistry = new ();

    public void AddPlatform(int id, Vector3 position, PlatformVisuals visualsPrefab) {
        var vehicleVisuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);        
        visualsRegistry[id] = vehicleVisuals;
    }

    public void UpdatePlatformPose(int id, VehicleState vehicleState) {
        var vehicleVisuals = visualsRegistry[id];
        vehicleVisuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        vehicleVisuals.SetFrontAxis(vehicleState.frontAxis);
        vehicleVisuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void RemovePlatform(int id) {
        var visuals = visualsRegistry[id];
        GameObject.Destroy(visuals.gameObject);
        visualsRegistry.Remove(id);
    }
}