using System;
using System.Collections.Generic;

using UnityEngine;


public class PlayerView {

    private readonly List<VehicleVisuals> vehicleVisualsRegistry = new ();
    private readonly Dictionary<int, GameObject> rewardVisualsRegistry = new ();

    public PlayerView() {
    }

    public int AddVehicle(Vector3 position, VehicleVisuals visualsPrefab) {
        var vehicleVisuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);        
        vehicleVisualsRegistry.Add(vehicleVisuals);
        return vehicleVisualsRegistry.Count - 1;
    }

    public void UpdateVehiclePose(int vehicleIndex, VehicleState vehicleState) {
        var vehicleVisuals = vehicleVisualsRegistry[vehicleIndex];
        vehicleVisuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        vehicleVisuals.SetFrontAxis(vehicleState.frontAxis);
        vehicleVisuals.SetRearAxis(vehicleState.rearAxis);
        // vehicleVisuals.SetTowingTongueRotation(towingTonguePose);
    }

    public void SpawnReward(int id, Vector3 position, GameObject rewardVisualsPrefab) {
        var visuals = GameObject.Instantiate(rewardVisualsPrefab, position, Quaternion.identity);
        rewardVisualsRegistry[id] = visuals;
    }

    public void DespawnReward(int id) {
        var visuals = rewardVisualsRegistry[id];
        GameObject.Destroy(visuals);
        rewardVisualsRegistry.Remove(id);
    }

}