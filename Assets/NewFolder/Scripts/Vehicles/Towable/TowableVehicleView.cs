using System.Collections.Generic;

using UnityEngine;

public class TowableVehicleView {
    private readonly Dictionary<TowableVehicleId, TowableVehicleVisuals> visualsRegistry = new ();

    public void AddVehicle(TowableVehicleId id, Vector3 position, TowableVehicleVisuals visualsPrefab) {
        var vehicleVisuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);        
        visualsRegistry[id] = vehicleVisuals;
    }

    public void UpdateVehiclePose(TowableVehicleId id, VehicleState vehicleState) {
        var vehicleVisuals = visualsRegistry[id];
        vehicleVisuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        vehicleVisuals.SetFrontAxis(vehicleState.frontAxis);
        vehicleVisuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void RemoveVehicle(TowableVehicleId id) {
        var visuals = visualsRegistry[id];
        GameObject.Destroy(visuals.gameObject);
        visualsRegistry.Remove(id);
    }
}