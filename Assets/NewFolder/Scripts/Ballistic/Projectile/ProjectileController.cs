using System;
using System.Collections.Generic;

using UnityEngine;

public class ProjectileController {

    private readonly ProjectileView view;
    private readonly CombatSystem combatSystem;
    private readonly PhysicsService physicsService;

    private int idCounter = 0;
    private readonly Dictionary<int, ProjectileModel> registry = new ();
    private readonly List<int> removeBuffer = new(16);

    public ProjectileController(CombatSystem combatSystem, ProjectileView view, PhysicsService physicsService) {
        this.combatSystem = combatSystem;
        this.view = view;
        this.physicsService = physicsService;
    }

    public void Create(int shooterId, ProjectilePrototype prototype, Orientation orientation) {
        var nextId = ++idCounter;
        var model = new ProjectileModel(nextId, prototype.config, shooterId);
        registry[nextId] = model;

        model.Position = orientation.origin;
        model.Velocity = orientation.direction * prototype.config.speed;
        model.SpawnTime = Time.time;

        view.ShowBulletShoot(model.Id, orientation.origin, model.Velocity, 
            prototype.config.style, prototype.config.shootAudioClips);
    }

    public void Update() {
        MoveProjectiles();
        UpdateProjectileHits();
        ValidateProjectils();
        FilterDeadProjectiles();
    }

    private void MoveProjectiles() {
        foreach (var model in registry.Values) {
            model.Position += model.Velocity * Time.deltaTime;
        }
    }

    private void UpdateProjectileHits() {   
        foreach (var projectile in registry.Values) {
            if (projectile.IsDead)
                continue;

            if (combatSystem.ApplyProjectileDamage(projectile.ShooterCombatId, projectile.Position, projectile.Velocity, projectile.Config.damage, out var hitDirection)) {
                projectile.IsDead = true;
                view.ShowBulletCrash(projectile.Id, projectile.Position, projectile.Config.impactAudioClips, projectile.Config.impactParticlesPrefab, hitDirection);
            }
        }
    }

    private void ValidateProjectils() {
        foreach (var projectile in registry.Values) {
            if (!projectile.IsDead && projectile.SpawnTime + projectile.Config.lifetime < Time.time) {
                projectile.IsDead = true;
                view.ShowBulletDisappear(projectile.Id);
            }
            if (!projectile.IsDead) {
                var projectileRay = new Ray(projectile.Position, projectile.Velocity);
                var projectileHitCheckDistance = 0.5f;
                if (physicsService.RaycastEnvironment(projectileRay, projectileHitCheckDistance, out var position, out var normal)) {
                    view.ShowBulletCrash(projectile.Id, position, projectile.Config.impactAudioClips, projectile.Config.impactParticlesPrefab, normal);
                    projectile.IsDead = true;
                }
            }
        }
    }

    private void FilterDeadProjectiles() {
        removeBuffer.Clear();
        foreach (var model in registry.Values) {
            if (model.IsDead) {
                removeBuffer.Add(model.Id);
            }
        }
        
        foreach (var id in removeBuffer) {
            registry.Remove(id);
        }
    }
}