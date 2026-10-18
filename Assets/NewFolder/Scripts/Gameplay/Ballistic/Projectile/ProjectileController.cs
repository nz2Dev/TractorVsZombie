using System;
using System.Collections.Generic;


using UnityEngine;

public class ProjectileController {

    private readonly ProjectileView view;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly HitboxSystem hitboxSystem;
    private readonly CollisionService collisionService;
    private readonly EntityMapping entityMapping;

    private int idCounter = 0;
    private readonly Dictionary<int, ProjectileModel> registry = new ();
    private readonly List<int> removeBuffer = new(16);

    public ProjectileController(HealthSystem healthSystem, FactionRegistry factionRegistry, ProjectileView view, HitboxSystem hitboxSystem, EntityMapping entityMapping, CollisionService collisionService) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.view = view;
        this.hitboxSystem = hitboxSystem;
        this.entityMapping = entityMapping;
        this.collisionService = collisionService;
    }

    public void Create(FactionId shooterFactionId, ProjectilePrototype prototype, Orientation orientation) {
        var shootIsAlie = factionRegistry.ReadState(shooterFactionId).alie;

        var nextId = ++idCounter;
        var model = new ProjectileModel(nextId, prototype.config, shooterFactionId, shootIsAlie);
        registry[nextId] = model;

        model.Position = orientation.origin;
        model.Velocity = orientation.direction * prototype.config.speed;
        model.SpawnTime = Time.time;

        view.SetupShooter(shooterFactionId.Value, prototype.shootAudioSourcePrefab, prototype.crashAudioSourcePrefab);
        view.ShowBulletShoot(shooterFactionId.Value, model.Id, orientation.origin, model.Velocity,
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

            var ray = new Ray(projectile.Position, projectile.Velocity);
            var hitCheckDistance = projectile.Velocity.magnitude * Time.fixedDeltaTime;
            if (hitboxSystem.Raycast(ray, hitCheckDistance, !projectile.ShooterAlie, out var hitboxId, out var hitInfo)) {
                projectile.IsDead = true;

                var surface = ContactSurface.None;
                if (entityMapping.TryFindByHitboxId(hitboxId, out var components)) {
                    var healthState = healthSystem.ReadState(components.healthId.Value);
                    surface = healthState.surface;

                    healthSystem.DealDamage(components.healthId.Value, new DamageInput {
                        damageSource = projectile.Position,
                        damageType = DamageType.Projectile,
                        damage = projectile.Config.damage
                    });
                }

                view.ShowBulletCrash(projectile.ShooterFactionId.Value, projectile.Id, projectile.Position,
                    projectile.Config.GetImapctAudioClips(surface), projectile.Config.GetImpactParticlesPrefab(surface), hitInfo.normal);
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
                if (collisionService.RaycastEnvironment(projectileRay, projectile.Velocity.magnitude * Time.fixedDeltaTime, out var hitInfo)) {
                    view.ShowBulletCrash(projectile.ShooterFactionId.Value, projectile.Id, hitInfo.point, projectile.Config.impactAudioClips, projectile.Config.impactParticlesPrefab, hitInfo.normal);
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
