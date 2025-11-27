using System.Collections.Generic;

using UnityEngine;

public class MotorVehicleView {
    private readonly Dictionary<MotorVehicleId, MotorVehicleVisuals> visualsRegistry = new ();

    public void AddVehicle(MotorVehicleId vehicleId, Vector3 position, MotorVehicleVisuals visualsPrefab) {
        var vehicleVisuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);        
        visualsRegistry[vehicleId] = vehicleVisuals;
    }

    public void UpdateVehiclePose(MotorVehicleId vehicleId, VehicleState vehicleState) {
        var vehicleVisuals = visualsRegistry[vehicleId];
        vehicleVisuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        vehicleVisuals.SetFrontAxis(vehicleState.frontAxis);
        vehicleVisuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void RemoveVehicle(MotorVehicleId vehicleId) {
        var visuals = visualsRegistry[vehicleId];
        GameObject.Destroy(visuals.gameObject);
        visualsRegistry.Remove(vehicleId);
    }
}