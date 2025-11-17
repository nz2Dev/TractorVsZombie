using System;
using System.Collections.Generic;

using UnityEngine;


public class VehicleView {

    private readonly List<VehicleVisuals> visualsRegistry = new ();

    public VehicleView() {
    }

    public int AddVehicle(Vector3 position, VehicleVisuals visualsPrefab) {
        var vehicleVisuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);        
        visualsRegistry.Add(vehicleVisuals);
        return visualsRegistry.Count - 1;
    }

    public void UpdateVehiclePose(int vehicleIndex, VehicleState vehicleState) {
        var vehicleVisuals = visualsRegistry[vehicleIndex];
        vehicleVisuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        vehicleVisuals.SetFrontAxis(vehicleState.frontAxis);
        vehicleVisuals.SetRearAxis(vehicleState.rearAxis);
        // vehicleVisuals.SetTowingTongueRotation(towingTonguePose);
    }

}