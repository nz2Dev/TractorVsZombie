using System;
using System.Collections.Generic;

using UnityEngine;

public class WeaponView {
    
    private readonly TurelVisuals turelVisualsPrefab;
    private readonly RocketLauncherVisuals rocketLauncherVisualsPrefab;

    private readonly Dictionary<int, TurelVisuals> turelVisualRegistry = new ();
    private readonly Dictionary<int, RocketLauncherVisuals> launcherVisualsRegistry = new ();

    public WeaponView(TurelVisuals turelVisualsPrefab, RocketLauncherVisuals rocketLauncherVisualsPrefab) {
        this.turelVisualsPrefab = turelVisualsPrefab;
        this.rocketLauncherVisualsPrefab = rocketLauncherVisualsPrefab;
    }

    public void AddTurel(int turelId, Vector3 position) {
        turelVisualRegistry[turelId] = GameObject.Instantiate(turelVisualsPrefab, position, Quaternion.identity);
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

    internal void AddRocketLauncher(int launcherId, Vector3 position) {
        launcherVisualsRegistry[launcherId] = GameObject.Instantiate(rocketLauncherVisualsPrefab, position, Quaternion.identity);
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

}