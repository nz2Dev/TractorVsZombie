using System;
using System.Collections.Generic;

using UnityEngine;

public class WeaponView {
    
    private readonly TurelVisuals turelVisualsPrefab;

    private readonly Dictionary<int, TurelVisuals> turelVisualRegistry = new ();

    public WeaponView(TurelVisuals turelVisualsPrefab) {
        this.turelVisualsPrefab = turelVisualsPrefab;
    }

    public void AddTurel(int turelId, Vector3 position) {
        turelVisualRegistry[turelId] = GameObject.Instantiate(turelVisualsPrefab, position, Quaternion.identity);
    }

    public void UpdateTurelOrientation(int turelId, Vector3 lookVector) {
        var turelVisuals = turelVisualRegistry[turelId];
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
}