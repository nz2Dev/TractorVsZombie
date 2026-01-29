using System.Collections.Generic;

using UnityEngine;

public class ProjectileController {

    private readonly ProjectileView view;
    private readonly SoundManager soundManager;
    private readonly CombatSystem combatSystem;
    private readonly Dictionary<int, ProjectileModel> models = new ();
    
    private int idCounter = 0;

    public ProjectileController(CombatSystem combatSystem, SoundManager soundManager, ProjectileView view) {
        this.combatSystem = combatSystem;
        this.soundManager = soundManager;
        this.view = view;
    }

    public void SpawnBulletProjectile(int shooterId, Bullet bullet, ProjectileConfig config) {
        var nextId = ++idCounter;
        var model = new ProjectileModel(nextId, shooterId, bullet.firePoint, bullet.velocity, Time.time, config);
        models[nextId] = model;
        view.ShowBulletShoot(model.Id, bullet.firePoint, bullet.velocity);
        soundManager.PlayEffect(bullet.firePoint, config.shootAudioClips);
    }

    public void Update() {
        MoveProjectiles();
        UpdateProjectileHits();
        FilterDeadProjectiles();
    }

    private void MoveProjectiles() {
        foreach (var projectile in models.Values) {
            projectile.Move(Time.deltaTime);
        }
    }

    private void UpdateProjectileHits() {   
        foreach (var projectile in models.Values) {
            if (projectile.IsAged)
                continue;

            if (combatSystem.ApplyProjectileDamage(projectile.ShooterId, projectile.Position, projectile.Velocity, projectile.Config.damage)) {
                projectile.Kill();
                view.ShowBulletCrash(projectile.Id);
            }
        }
    }

    private void FilterDeadProjectiles() {
        foreach (var projectile in models.Values) {
            if (projectile.IsAged) {
                view.ShowBulletDisappear(projectile.Id);
            }
        }
    }
}