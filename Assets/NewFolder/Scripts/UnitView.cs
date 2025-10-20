using System;
using System.Collections.Generic;
using System.Data.Common;

using UnityEngine;
using UnityEngine.Assertions;

public class UnitView {

    private readonly Dictionary<int, UnitVisuals> unitVisuals = new Dictionary<int, UnitVisuals>();
    int vehicleViewIdCounter;
    private readonly Dictionary<int, VehicleVisuals> visualsRegistry = new ();
    private readonly Dictionary<int, TurelVisuals> turelVisualRegistry = new ();
    private readonly Dictionary<int, RocketLauncherVisuals> launcherVisualsRegistry = new ();

    private readonly Transform container;
    private readonly UnitVisuals visualsPrefab;
    private readonly TurelVisuals turelVisualsPrefab;
    private readonly RocketLauncherVisuals rocketLauncherVisualsPrefab;

    public UnitView(UnitVisuals visualsPrefab, Transform container, TurelVisuals turelVisualsPrefab, RocketLauncherVisuals rocketLauncherVisualsPrefab) {
        this.visualsPrefab = visualsPrefab;
        this.container = container;
        this.turelVisualsPrefab = turelVisualsPrefab;
        this.rocketLauncherVisualsPrefab = rocketLauncherVisualsPrefab;
    }

    public void AddUnit(int unitId, Vector3 position) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
        unitVisuals.Add(unitId, visuals);
    }

    public void UpdateUnitPositionAndRotation(int unitId, Vector3 position, Quaternion rotation) {
        unitVisuals[unitId].UpdatePositionAndRotation(position, rotation);
    }

    internal void ShowTakeHit(int unitId) {
        unitVisuals[unitId].PlayTakeHit();
    }

    public void ShowDirectFrontAttack(int unitId) {
        unitVisuals[unitId].PlayDirectAttackAnimation();
    }

    public void ShowDeathByProjectile(int unitId, Vector3 damageSourcePosition, bool blownAway) {
        var visuals = unitVisuals[unitId];
        if (blownAway) {
            visuals.RotateAway(damageSourcePosition);
            visuals.PlayPushedAwayDeathAnimation();
        } else {
            visuals.PlayDisolveAnimation();
        }
    }

    internal void ShowDisolveDeath(int unitId) {
        var visuals = unitVisuals[unitId];
        visuals.PlayDisolveAnimation();
    }

    public void RemoveUnit(int unitId) {
        var visuals = unitVisuals[unitId];
        visuals.DestroySelfOnIdle();
        unitVisuals.Remove(unitId);
    }

    public int AddUnitVehicle(Vector3 position, VehiclePhysicsData physicsData, UnitVehicleData.VisualsData visualsData) {
        var vehicleVisuals = new VehicleVisuals(position, container);
        vehicleVisuals.AddBaseGeometry(visualsData.baseGeometry);
        
        for (int i = 0; i < physicsData.wheelAxisDatas.Length; i++) {
            var wheelAxis = physicsData.wheelAxisDatas[i];
            vehicleVisuals.AddWheelAxisGeometries(
                visualsData.wheelGeometry, 
                wheelAxis.forwardOffset,
                wheelAxis.upOffset,
                wheelAxis.halfLength,
                wheelAxis.radius
            );
        }
        
        var nextVehicleViewId = vehicleViewIdCounter++;
        visualsRegistry[nextVehicleViewId] = vehicleVisuals;
        return nextVehicleViewId;
    }

    internal void RemoveVehicleView(int viewId) {
        var vehicleVisuals = visualsRegistry[viewId];
        vehicleVisuals.DestroySelf();
        visualsRegistry.Remove(viewId);
        if (turelVisualRegistry.TryGetValue(viewId, out var turelVisuals)) {
            turelVisuals.DestroySelf();
            turelVisualRegistry.Remove(viewId);
        }
        if (launcherVisualsRegistry.TryGetValue(viewId, out var launcherVisuals)) {
            launcherVisuals.DestroySelf();
            launcherVisualsRegistry.Remove(viewId);
        }
    }

    public void UpdateVehiclePose(int vehicleIndex, VehicleBodyPose vehiclePose) {
        var vehicleVisuals = visualsRegistry[vehicleIndex];
        vehicleVisuals.SetPositionAndRotation(vehiclePose);
    }

    public void UpdateWheelAxisPose(int vehicleIndex, int axisIndex, WheelAxisPose wheelAxisPose) {
        var vehicleVisuals = visualsRegistry[vehicleIndex];
        vehicleVisuals.SetAxisPositionAndRotation(axisIndex, wheelAxisPose);
    }

    internal void SetTurelWeapon(int viewId, Vector3 position) {
        turelVisualRegistry[viewId] = GameObject.Instantiate(turelVisualsPrefab, position, Quaternion.identity);
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

    internal void SetRocketLauncherWeapon(int viewId, Vector3 position) {
        launcherVisualsRegistry[viewId] = GameObject.Instantiate(rocketLauncherVisualsPrefab, position, Quaternion.identity);
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