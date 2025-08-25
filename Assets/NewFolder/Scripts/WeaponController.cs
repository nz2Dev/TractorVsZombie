
using System;
using System.Collections.Generic;

using UnityEngine;

public class WeaponController {
    
    private readonly WeaponView view;
    private readonly ICombatService combatService;
    private readonly TurelConfig turelConfig;
    
    private Turel turel;
    private int combatAgentId;
    private int nextProjectileId = 1;
    private List<Projectile> bulletProjectiles = new List<Projectile>();
    private List<int> crashedProjectileIndexes = new List<int>();

    public WeaponController(WeaponView weaponView, ICombatService interactionService, TurelConfig turelConfig) {
        this.view = weaponView;
        this.combatService = interactionService;
        this.turelConfig = turelConfig;
    }

    public void Init() {
        turel = new Turel(1, Vector3.zero, turelConfig);
        combatAgentId = combatService.RegisterAgent(turel.Position);
        view.AddTurel(turel.Position);
    }

    public void Update() {
        UpdateProjectilesMovement(Time.deltaTime);
        UpdateProjectilesCombat();
        OperateTurel();
        UpdateTurelView();
    }

    private void OperateTurel() {
        if (combatService.GetClosestEnemyAgentInRange(combatAgentId, 20, out var closestEnemyAgent)) {
            turel.Aim(Time.deltaTime, closestEnemyAgent.position);
        }

        if (turel.Fire(Time.time, out var bullet)) {
            SpawnBulletProjectile(bullet);
        }
    }

    private void UpdateTurelView() {
        view.UpdateTurelOrientation(turel.AimForward);
    }

    private void SpawnBulletProjectile(Bullet bullet) {
        var projectile = new Projectile { id = nextProjectileId++, position = bullet.firePoint, velocity = bullet.velocity };
        bulletProjectiles.Add(projectile);
        view.ShowBulletShoot(projectile.id, projectile.velocity);
    }

    private void UpdateProjectilesMovement(float deltaTime) {
        for (int turelProjectileIndex = 0; turelProjectileIndex < bulletProjectiles.Count; turelProjectileIndex++) {
            var projectile = bulletProjectiles[turelProjectileIndex];
            projectile.Move(deltaTime);
        }
    }

    private void UpdateProjectilesCombat() {   
        crashedProjectileIndexes.Clear();
        for (int i = 0; i < bulletProjectiles.Count; i++) {
            var projectile = bulletProjectiles[i];
            if (combatService.ApplyProjectileDamage(combatAgentId, projectile.position, projectile.velocity, turel.BulletDamage)) {
                crashedProjectileIndexes.Add(i);
            }
        }

        foreach (var projectileIndex in crashedProjectileIndexes) {
            var projectile = bulletProjectiles[projectileIndex];
            bulletProjectiles.RemoveAt(projectileIndex);
            view.ShowBulletCrash(projectile.id);
        }
    }

#region debug
#if UNITY_EDITOR
    public void OnDrawGizmos() {
        Gizmos.color = Color.white;
        foreach (var projectile in bulletProjectiles) {
            Gizmos.DrawWireSphere(projectile.position, 0.3f);
        }
    }
#endif
#endregion

}