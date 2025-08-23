
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

public class WeaponController {
    
    private readonly WeaponView view;
    private readonly ICombatService combatService;
    
    private Turel turel;
    private int combatAgentId;
    private List<Projectile> projectiles = new List<Projectile>();

    public WeaponController(WeaponView weaponView, ICombatService interactionService) {
        this.view = weaponView;
        this.combatService = interactionService;
    }

    public void Init() {
        turel = new Turel(1);
        combatAgentId = combatService.RegisterAgent(turel.Position);
        view.AddTurel(turel.Position);
    }

    public void FixedUpdate() {
        UpdateProjectilesMovement(Time.fixedDeltaTime);
    }

    private void UpdateProjectilesMovement(float deltaTime) {
        for (int turelProjectileIndex = 0; turelProjectileIndex < projectiles.Count; turelProjectileIndex++) {
            var projectile = projectiles[turelProjectileIndex];
            projectile.Move(deltaTime);
            combatService.UpdateProjectile(combatAgentId, turelProjectileIndex, projectile.position);
        }
    }

    public void Update() {
        turel.Aim(new Vector3(0, 1, 10));
        if (turel.Shoot(Time.time, out var shootRay)) {
            SpawnTurelBullet(shootRay);
        }

        FilterDestroyedBullets();
    }

    private void SpawnTurelBullet(RayDamage rayDamage) {
        projectiles.Add(new Projectile { position = rayDamage.sourcePosition, velocity = rayDamage.rayDirection });
        int projectileOrderNumber = combatService.RegisterProjectile(combatAgentId, rayDamage.sourcePosition, rayDamage.amount);
        view.ShowBulletShoot(projectileOrderNumber);
    }

    private void FilterDestroyedBullets() {
        var destroyedProjectileEvents = combatService.GetDestroyedProjectilesEventsCount(combatAgentId);
        for (int eventIndex = 0; eventIndex < destroyedProjectileEvents; eventIndex++) {
            var projectileIndex = combatService.GetDestroyedProjectileIndex(combatAgentId, eventIndex);
            view.ShowBulletCrash(projectileIndex);
        }
    }

}