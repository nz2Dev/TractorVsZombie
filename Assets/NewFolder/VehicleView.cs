using System.Collections.Generic;

using UnityEngine;


public class VehicleView {

    private readonly Transform container;
    private readonly List<VehicleVisuals> visualsRegistry = new ();

    public VehicleView(Transform container) {
        this.container = container;
    }

    public void AddVehicle(GameObject baseGeometryPrefab, Transform baseGeometryFit, GameObject wheelGeometry, Transform wheelGeometryFit, WheelAxisData[] wheelAxisDatas) {
        var vehicleVisuals = new VehicleVisuals(container);
        vehicleVisuals.AddBaseGeometry(baseGeometryPrefab, baseGeometryFit);
        foreach (var wheelAxis in wheelAxisDatas)
            vehicleVisuals.AddWheelAxisGeometries(
                wheelGeometry, 
                wheelGeometryFit, 
                wheelAxis.forwardOffset,
                wheelAxis.upOffset,
                wheelAxis.halfLength,
                wheelAxis.radius
            );
        
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

}