
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

    public void FixedUpdate() {}

    public void Update() {
        UpdateProjectilesMovement(Time.deltaTime);
        UpdateProjectilesCombat();

        if (combatService.GetClosestEnemyAgentInRange(combatAgentId, 20, out var closestEnemyAgent)) {
            turel.Aim(Time.deltaTime, closestEnemyAgent.position);
        }

        if (turel.Shoot(Time.time, out var shootRay)) {
            SpawnTurelBullet(shootRay);
        }
    }

    private int nextProjectileId = 1;

    private void SpawnTurelBullet(RayDamage rayDamage) {
        var projectile = new Projectile { id = nextProjectileId++, position = rayDamage.sourcePosition, velocity = rayDamage.velocity };
        projectiles.Add(projectile);
        view.ShowBulletShoot(projectile.id, projectile.velocity);
    }

    private void UpdateProjectilesMovement(float deltaTime) {
        for (int turelProjectileIndex = 0; turelProjectileIndex < projectiles.Count; turelProjectileIndex++) {
            var projectile = projectiles[turelProjectileIndex];
            projectile.Move(deltaTime);
        }
    }

    private List<int> crashedProjectileIndexes = new List<int>();

    private void UpdateProjectilesCombat() {   
        crashedProjectileIndexes.Clear();
        for (int turelProjectileIndex = 0; turelProjectileIndex < projectiles.Count; turelProjectileIndex++) {
            var projectile = projectiles[turelProjectileIndex];
            if (combatService.ApplyProjectileDamage(combatAgentId, projectile.position, projectile.velocity, turel.Damage)) {
                crashedProjectileIndexes.Add(turelProjectileIndex);
            }
        }

        foreach (var projectileIndex in crashedProjectileIndexes) {
            var projectile = projectiles[projectileIndex];
            projectiles.RemoveAt(projectileIndex);
            view.ShowBulletCrash(projectile.id);
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