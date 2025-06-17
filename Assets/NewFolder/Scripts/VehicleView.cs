using System;
using System.Collections.Generic;

using UnityEngine;


public class VehicleView {

    private readonly Transform container;
    private readonly List<VehicleVisuals> visualsRegistry = new ();

    public VehicleView(Transform container) {
        this.container = container;
    }

    public void AddVehicle(Vector3 position, GameObject baseGeometryPrefab, GameObject wheelGeometry, GameObject towingBodyGeometry, WheelAxisData[] wheelAxisDatas, TowingWheelAxisData? towingAxis) {
        var vehicleVisuals = new VehicleVisuals(position, container);
        vehicleVisuals.AddBaseGeometry(baseGeometryPrefab);
        
        foreach (var wheelAxis in wheelAxisDatas)
            vehicleVisuals.AddWheelAxisGeometries(
                wheelGeometry, 
                wheelAxis.forwardOffset,
                wheelAxis.upOffset,
                wheelAxis.halfLength,
                wheelAxis.radius
            );
        
        if (towingAxis.HasValue) {
            vehicleVisuals.AddTowingWheelAxisGeometries(
                wheelGeometry, 
                towingBodyGeometry,
                towingAxis.Value.forwardOffset,
                towingAxis.Value.upOffset,
                towingAxis.Value.halfLength,
                towingAxis.Value.radius,
                towingAxis.Value.towingBodyLength
            );
        }
        
        visualsRegistry.Add(vehicleVisuals);
    }

    public void UpdateVehiclePose(int vehicleIndex, VehiclePose vehiclePose) {
        var vehicleVisuals = visualsRegistry[vehicleIndex];
        vehicleVisuals.SetPositionAndRotation(vehiclePose);
    }

    public void UpdateWheelAxisPose(int vehicleIndex, int axisIndex, WheelAxisPose wheelAxisPose) {
        var vehicleVisuals = visualsRegistry[vehicleIndex];
        vehicleVisuals.SetAxisPositionAndRotation(axisIndex, wheelAxisPose);
    }

    internal void UpdateTowingWheelAxisPose(int vehicleIndex, TowingWheelAxisPose towingWheelAxisPose) {
        var vehicleVisuals = visualsRegistry[vehicleIndex];
        vehicleVisuals.SetTowingAxisPositionAndRotation(towingWheelAxisPose);
    }
}