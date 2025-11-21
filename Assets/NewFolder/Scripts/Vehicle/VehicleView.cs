using System;
using System.Collections.Generic;

using UnityEngine;

public class VehicleView {

    private readonly Dictionary<int, VehicleVisuals> visualsRegistry = new ();

    public void AddVehicle(int vehicleId, Vector3 position, VehicleVisuals visualsPrefab) {
        var vehicleVisuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);        
        visualsRegistry[vehicleId] = vehicleVisuals;
    }

    public void UpdateVehiclePose(int vehicleId, VehicleState vehicleState) {
        var vehicleVisuals = visualsRegistry[vehicleId];
        vehicleVisuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        vehicleVisuals.SetFrontAxis(vehicleState.frontAxis);
        vehicleVisuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void RemoveVehicle(int vehicleId) {
        var visuals = visualsRegistry[vehicleId];
        GameObject.Destroy(visuals.gameObject);
        visualsRegistry.Remove(vehicleId);
    }
}