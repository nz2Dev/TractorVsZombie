using System;
using System.Collections.Generic;

using UnityEngine;


public class VehicleView {

    private readonly Transform container;
    private readonly List<VehicleVisuals> visualsRegistry = new ();

    public VehicleView(Transform container) {
        this.container = container;
    }

    public void AddVehicle(Vector3 position, VehiclePhysicsData physicsData, VehicleVisualsData visualsData) {
        var vehicleVisuals = new VehicleVisuals(position, container);
        vehicleVisuals.AddBaseGeometry(visualsData.baseGeometry);
        
        var hasTowingTongue = physicsData.towingTongueLength > 0;
        for (int i = 0; i < physicsData.wheelAxisDatas.Length; i++) {
            bool isLastAxis = i == physicsData.wheelAxisDatas.Length - 1;
            var wheelAxis = physicsData.wheelAxisDatas[i];
            if (hasTowingTongue && isLastAxis) {
                vehicleVisuals.AddTowingWheelAxisGeometries(
                    visualsData.wheelGeometry, 
                    visualsData.towingBodyGeometry,
                    wheelAxis.forwardOffset,
                    wheelAxis.upOffset,
                    wheelAxis.halfLength,
                    wheelAxis.radius,
                    physicsData.towingTongueLength
                );
            } else {
                vehicleVisuals.AddWheelAxisGeometries(
                    visualsData.wheelGeometry, 
                    wheelAxis.forwardOffset,
                    wheelAxis.upOffset,
                    wheelAxis.halfLength,
                    wheelAxis.radius
                );
            }
        }
        
        visualsRegistry.Add(vehicleVisuals);
    }

    public void UpdateVehiclePose(int vehicleIndex, VehicleBodyPose vehiclePose) {
        var vehicleVisuals = visualsRegistry[vehicleIndex];
        vehicleVisuals.SetPositionAndRotation(vehiclePose);
    }

    public void UpdateWheelAxisPose(int vehicleIndex, int axisIndex, WheelAxisPose wheelAxisPose) {
        var vehicleVisuals = visualsRegistry[vehicleIndex];
        vehicleVisuals.SetAxisPositionAndRotation(axisIndex, wheelAxisPose);
    }

    internal void UpdateTowingTonguePose(int vehicleIndex, Quaternion towingTonguePose) {
        var vehicleVisuals = visualsRegistry[vehicleIndex];
        vehicleVisuals.SetTowingTongueRotation(towingTonguePose);
    }
}