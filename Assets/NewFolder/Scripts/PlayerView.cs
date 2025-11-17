using System;
using System.Collections.Generic;

using UnityEngine;


public class PlayerView {

    private readonly List<VehicleVisuals> vehicleVisualsRegistry = new ();
    private readonly Dictionary<int, TurelVisuals> turelVisualRegistry = new ();
    private readonly Dictionary<int, RocketLauncherVisuals> launcherVisualsRegistry = new ();
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

    public void AddTurel(int turelId, Vector3 position, TurelVisuals visualsPrefab) {
        turelVisualRegistry[turelId] = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
    }

    public void UpdateTurelOrientation(int turelId, Vector3 position, Vector3 lookVector) {
        var turelVisuals = turelVisualRegistry[turelId];
        turelVisuals.UpdatePosition(position);
        turelVisuals.UpdateAim(lookVector);
    }

    internal void ShowBulletShoot(int turelId, int projectileId, Vector3 velocity) {
        var turelVisuals = turelVisualRegistry[turelId];
        turelVisuals.ShowBulletFire(projectileId, velocity);
    }

    internal void ShowBulletCrash(int turelId, int projectileIndex) {
        var turelVisuals = turelVisualRegistry[turelId];
        turelVisuals.KillBulletFire(projectileIndex);
    }

    internal void ShowBulletDisappear(int turelId, int id) {
        var turelVisuals = turelVisualRegistry[turelId];
        turelVisuals.KillBulletFire(id);
    }

    internal void AddRocketLauncher(int launcherId, Vector3 position, RocketLauncherVisuals visualsPrefab) {
        launcherVisualsRegistry[launcherId] = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
    }

    internal void ShowRocketFly(int launcherId, int rocketId, RocketTrajectory trajectory, float rocketFlyDuration) {
        var launcherVisuals = launcherVisualsRegistry[launcherId];
        launcherVisuals.ShowRocketFly(rocketId, trajectory, rocketFlyDuration);
    }

    internal void ShowRocketExplosion(int launcherId, int rocketId) {
        var launcherVisuals = launcherVisualsRegistry[launcherId];
        launcherVisuals.ShowRocketExplosion(rocketId);
    }

    internal void UpdateRocketLauncherOrientation(int launcherId, Vector3 position, Vector3 aimPoint, float aimHeight) {
        var launcherVisuals = launcherVisualsRegistry[launcherId];
        launcherVisuals.UpdatePosition(position);
        launcherVisuals.OrientLauncherTowardAim(aimPoint, aimHeight);
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