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

        view.SetupShooter(shooterId, prototype.shootAudioSourcePrefab, prototype.crashAudioSourcePrefab);
        view.ShowBulletShoot(shooterId, model.Id, orientation.origin, model.Velocity, 
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

            var projectileHitCheckDistance = projectile.Velocity.magnitude * Time.fixedDeltaTime;
            if (combatSystem.ApplyProjectileDamage(projectile.ShooterCombatId, projectile.Position, projectile.Velocity, projectileHitCheckDistance, 
                projectile.Config.damage, out var hitDirection, out var hitSurface)) {
                projectile.IsDead = true;
                
                AudioClip[] impactAudioClips;
                ParticleSystem impactVFXPrefab;
                if (hitSurface == ContactSurface.Metal) {
                    impactAudioClips = projectile.Config.metalImpactAudioClips;
                    impactVFXPrefab = projectile.Config.metalImpactParticlesPrefab;
                } else {
                    impactAudioClips = projectile.Config.softImpactAudioClips;
                    impactVFXPrefab = projectile.Config.softImpactParticlesPrefab;
                }
                
                view.ShowBulletCrash(projectile.ShooterCombatId, projectile.Id, projectile.Position, impactAudioClips, impactVFXPrefab, hitDirection);
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
                if (physicsService.RaycastEnvironment(projectileRay, projectile.Velocity.magnitude * Time.fixedDeltaTime, out var hitInfo)) {
                    view.ShowBulletCrash(projectile.ShooterCombatId, projectile.Id, hitInfo.point, projectile.Config.impactAudioClips, projectile.Config.impactParticlesPrefab, hitInfo.normal);
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