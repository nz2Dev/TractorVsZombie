
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
        turel.Aim(new Vector3(0, 1, 10));
        if (turel.Shoot(Time.time, out var shootRay)) {
            SpawnTurelBullet(shootRay);
        }
    }

    public void Update() {}

    private void SpawnTurelBullet(RayDamage rayDamage) {
        var projectile = new Projectile { position = rayDamage.sourcePosition, velocity = rayDamage.velocity };
        projectiles.Add(projectile);
        view.ShowBulletShoot(projectiles.Count, projectile.velocity);
    }

    private void UpdateProjectilesMovement(float deltaTime) {
        for (int turelProjectileIndex = 0; turelProjectileIndex < projectiles.Count; turelProjectileIndex++) {
            var projectile = projectiles[turelProjectileIndex];
            projectile.Move(deltaTime);
        }
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

#if UNITY_EDITOR
    public void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        foreach (var projectile in projectiles) {
            Gizmos.DrawWireSphere(projectile.position, 0.3f);
        }
    }
#endif

}