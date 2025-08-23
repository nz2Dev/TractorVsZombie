using System;

using UnityEngine;

public class WeaponView {
    
    private readonly TurelVisuals turelVisualsPrefab;

    private TurelVisuals turelVisuals;

    public WeaponView(TurelVisuals turelVisualsPrefab) {
        this.turelVisualsPrefab = turelVisualsPrefab;
    }

    public void AddTurel(Vector3 position) {
        turelVisuals = GameObject.Instantiate(turelVisualsPrefab, position, Quaternion.identity);
    }

    internal void ShowBulletShoot(int projectileOrderNumber, Vector3 velocity) {
        turelVisuals.ShowShootEffect(projectileOrderNumber, velocity);
    }

    internal void ShowBulletCrash(int projectileIndex) {
        turelVisuals.KillShootBullet(projectileIndex);
    }
}