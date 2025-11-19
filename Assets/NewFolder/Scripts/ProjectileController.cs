using System.Collections.Generic;

using UnityEngine;

public class ProjectileController {

    private readonly ProjectileView view;
    private readonly SoundManager soundManager;
    private readonly CombatService combatService;
    private readonly Dictionary<int, ProjectileModel> models = new ();
    
    private int idCounter = 0;

    public ProjectileController(CombatService combatService, SoundManager soundManager, ProjectileView view) {
        this.combatService = combatService;
        this.soundManager = soundManager;
        this.view = view;
    }

    public void Init() {
        view.Start();
    }

    public void SpawnBulletProjectile(int shooterId, Bullet bullet, AudioClip[] shootSFX) {
        var nextId = ++idCounter;
        var model = new ProjectileModel(nextId, shooterId, bullet.firePoint, bullet.velocity, Time.time, 5f);
        models[nextId] = model;
        view.ShowBulletShoot(model.Id, bullet.firePoint, bullet.velocity);
        soundManager.PlayEffect(bullet.firePoint, shootSFX);
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

            if (combatService.ApplyProjectileDamage(projectile.ShooterId, projectile.Position, projectile.Velocity, 1/*?*/)) {
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