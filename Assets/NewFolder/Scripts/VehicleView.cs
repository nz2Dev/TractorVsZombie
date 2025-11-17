using System;
using System.Collections.Generic;

using UnityEngine;


public class VehicleView {

    private readonly Transform container;
    private readonly List<VehicleVisuals> visualsRegistry = new ();

    public VehicleView(Transform container) {
        this.container = container;
    }

    public int AddVehicle(Vector3 position, GameObject baseGeometry, GameObject wheelGeometry) {
        var vehicleVisuals = new VehicleVisuals(position, container);
        vehicleVisuals.AddBaseGeometry(baseGeometry);
        
        vehicleVisuals.AddWheelAxisGeometries(wheelGeometry, .3f, .15f, .15f, .15f);
        vehicleVisuals.AddWheelAxisGeometries(wheelGeometry, -.3f, .15f, .15f, .15f);
        
        visualsRegistry.Add(vehicleVisuals);
        return visualsRegistry.Count - 1;
    }

    public void UpdateVehiclePose(int vehicleIndex, VehicleState vehicleState) {
        var vehicleVisuals = visualsRegistry[vehicleIndex];
        vehicleVisuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        vehicleVisuals.SetAxisPositionAndRotation(0, vehicleState.frontAxis);
        vehicleVisuals.SetAxisPositionAndRotation(1, vehicleState.rearAxis);
        // vehicleVisuals.SetTowingTongueRotation(towingTonguePose);
    }

}