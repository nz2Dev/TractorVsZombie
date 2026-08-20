using System;
using System.Collections.Generic;

using Combat;

using UnityEngine;

public class ProjectileController {

    private readonly ProjectileView view;
    private readonly CombatSystem combatSystem;
    private readonly RaycastService raycastService;
    private readonly InfantryController infantryController;

    private int idCounter = 0;
    private readonly Dictionary<int, ProjectileModel> registry = new ();
    private readonly List<int> removeBuffer = new(16);

    public ProjectileController(CombatSystem combatSystem, ProjectileView view, RaycastService raycastService, InfantryController infantryController) {
        this.combatSystem = combatSystem;
        this.view = view;
        this.raycastService = raycastService;
        this.infantryController = infantryController;
    }

    public void Create(CombatId shooterCombatId, ProjectilePrototype prototype, Orientation orientation) {
        var shootIsAlie = combatSystem.ReadState(shooterCombatId).alie;

        var nextId = ++idCounter;
        var model = new ProjectileModel(nextId, prototype.config, shooterCombatId, shootIsAlie);
        registry[nextId] = model;

        model.Position = orientation.origin;
        model.Velocity = orientation.direction * prototype.config.speed;
        model.SpawnTime = Time.time;

        view.SetupShooter(shooterCombatId.Value, prototype.shootAudioSourcePrefab, prototype.crashAudioSourcePrefab);
        view.ShowBulletShoot(shooterCombatId.Value, model.Id, orientation.origin, model.Velocity,
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
            var targetRaycastLayer = CombatSystem.GetRaycastLayerForFaction(!projectile.ShooterAlie);
            if (raycastService.Raycast(ray, hitCheckDistance, targetRaycastLayer, out var hitRaycastId, out var hitInfo)) {
                projectile.IsDead = true;

                var surface = Compatibility.ContactSurface.None;
                if (infantryController.TryFindByRaycastId(hitRaycastId, out var hitInfantryId)) {
                    var hitInfantry = infantryController.GetInfantryState(hitInfantryId);
                    var hitCombat = combatSystem.ReadState(hitInfantry.combatId);
                    surface = hitCombat.surface;

                    combatSystem.DealDamage(hitInfantry.combatId, new DamageInput {
                        damageSource = projectile.Position,
                        damageType = DamageType.Projectile,
                        damage = projectile.Config.damage
                    });
                }

                // TODO: search for Armor/Platform/Truck entities

                view.ShowBulletCrash(projectile.ShooterCombatId.Value, projectile.Id, projectile.Position, 
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
                if (raycastService.RaycastEnvironment(projectileRay, projectile.Velocity.magnitude * Time.fixedDeltaTime, out var hitInfo)) {
                    view.ShowBulletCrash(projectile.ShooterCombatId.Value, projectile.Id, hitInfo.point, projectile.Config.impactAudioClips, projectile.Config.impactParticlesPrefab, hitInfo.normal);
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
