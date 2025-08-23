
using System;
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
        UpdateProjectilesCombat();
    }

    private List<int> hitProjectileIndexesBuffer = new List<int>();

    private void UpdateProjectilesCombat() {   
        hitProjectileIndexesBuffer.Clear();
        for (int turelProjectileIndex = 0; turelProjectileIndex < projectiles.Count; turelProjectileIndex++) {
            var projectile = projectiles[turelProjectileIndex];
            if (combatService.ApplyProjectileDamage(combatAgentId, projectile.position, projectile.velocity, turel.Damage)) {
                hitProjectileIndexesBuffer.Add(turelProjectileIndex);
            }
        }

        foreach (var crashedBullet in hitProjectileIndexesBuffer) {
            projectiles.RemoveAt(crashedBullet);
            view.ShowBulletCrash(crashedBullet);
        }
    }

    private void UpdateProjectilesMovement(float deltaTime) {
        for (int turelProjectileIndex = 0; turelProjectileIndex < projectiles.Count; turelProjectileIndex++) {
            var projectile = projectiles[turelProjectileIndex];
            projectile.Move(deltaTime);
        }
    }

    public void Update() {
        turel.Aim(new Vector3(0, 1, 10));
        if (turel.Shoot(Time.time, out var shootRay)) {
            SpawnTurelBullet(shootRay);
        }
    }

    private void SpawnTurelBullet(RayDamage rayDamage) {
        projectiles.Add(new Projectile { position = rayDamage.sourcePosition, velocity = rayDamage.rayDirection });
        view.ShowBulletShoot(projectiles.Count);
    }

}