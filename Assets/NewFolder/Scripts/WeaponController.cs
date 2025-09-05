
using System;
using System.Collections.Generic;

using UnityEngine;

public class WeaponController {
    
    private readonly WeaponView view;
    private readonly ICombatService combatService;
    private readonly IProjectileService projectileService;
    private readonly TurelConfig turelConfig;
    
    private int turelIdCounter;
    private int turelsCombatGroupId;
    private readonly List<Turel> turels = new ();
    private readonly Dictionary<int, int> turelToCombatId = new ();
    private readonly Dictionary<int, int> turelToProjectileGroupId = new ();
    private readonly List<ProjectileState> projectilesStateBuffer = new (64);

    public WeaponController(WeaponView weaponView, ICombatService interactionService, TurelConfig turelConfig, IProjectileService projectileService) {
        this.view = weaponView;
        this.combatService = interactionService;
        this.turelConfig = turelConfig;
        this.projectileService = projectileService;
    }

    public void Init() {
        turelsCombatGroupId = combatService.AddGroup();

        SpawnTurel(Vector3.zero, turelConfig);
        
        int maxTurels = 5;
        float radius = 5;
        for (int i = 0; i < maxTurels; i++) {
            var placementAngle = (float) i / maxTurels * Mathf.PI * 2;
            
            Vector3 position = new Vector3(Mathf.Cos(placementAngle) * radius, 0, Mathf.Sin(placementAngle) * radius);
            SpawnTurel(position, turelConfig);
        }
    }

    public void Update() {
        FilterDeadProjectiles();
        UpdateProjectileHits();
        OperateTurels();
        UpdateTurelView();
    }

    private void SpawnTurel(Vector3 position, TurelConfig turelConfig) {
        var turelId = turelIdCounter++;
        var turel = new Turel(turelId, position, turelConfig);
        turels.Add(turel);
        
        var turelCombatId = combatService.RegisterAgent(turel.Position, groupId: turelsCombatGroupId);
        turelToCombatId[turel.Id] = turelCombatId;

        var turelProjectilesGroupId = projectileService.AddGroup();
        turelToProjectileGroupId[turel.Id] = turelProjectilesGroupId;
        
        view.AddTurel(turelId, turel.Position);
    }

    private void OperateTurels() {
        foreach (var turel in turels) {    
            var turelCombatId = turelToCombatId[turel.Id];
            
            if (combatService.GetClosestEnemyAgentInRange(turelCombatId, 20, out var closestEnemyAgent, excludeGroup: turelsCombatGroupId)) {
                var aimPoint = closestEnemyAgent.position + 0.5f * closestEnemyAgent.height * Vector3.up;
                turel.Aim(Time.deltaTime, aimPoint);
            }

            if (turel.Fire(Time.time, out var bullet)) {
                SpawnBulletProjectile(turel, bullet);
            }
        }
    }

    private void UpdateTurelView() {
        foreach (var turel in turels) {
            view.UpdateTurelOrientation(turel.Id, turel.GunForward);
        }
    }

    private void SpawnBulletProjectile(Turel turel, Bullet bullet) {
        var projectileGroupId = turelToProjectileGroupId[turel.Id];
        var projectileId = projectileService.CreateProjectile(projectileGroupId, bullet.firePoint, bullet.velocity, 5f);
        view.ShowBulletShoot(turel.Id, projectileId, bullet.velocity);
    }

    private void FilterDeadProjectiles() {
        foreach (var turel in turels) {
            var projectileGroup = turelToProjectileGroupId[turel.Id];
            projectilesStateBuffer.Clear();
            projectileService.GetGroupProjectiles(projectileGroup, projectilesStateBuffer);
            
            foreach (var projectileState in projectilesStateBuffer) {
                if (projectileState.isAged) {
                    view.ShowBulletDisappear(turel.Id, projectileState.id);
                }
            }
        }
    }

    private void UpdateProjectileHits() {   
        foreach (var turel in turels) {
            var projectileGroup = turelToProjectileGroupId[turel.Id];
            projectilesStateBuffer.Clear();
            projectileService.GetGroupProjectiles(projectileGroup, projectilesStateBuffer);

            var combatId = turelToCombatId[turel.Id];
            foreach (var projectileState in projectilesStateBuffer) {
                if (projectileState.isAged)
                    continue;

                if (combatService.ApplyProjectileDamage(combatId, projectileState.position, projectileState.velocity, turel.BulletDamage)) {
                    projectileService.KillProjectile(projectileState.id);
                    view.ShowBulletCrash(turel.Id, projectileState.id);
                }
            }
        }
    }
}