
using System;
using System.Collections.Generic;

using UnityEngine;

public class WeaponController {
    
    private readonly WeaponView view;
    private readonly ICombatService combatService;
    private readonly IProjectileService projectileService;
    private readonly TurelConfig turelConfig;
    private readonly RocketLauncherConfig launcherConfig;
    
    private int turelIdCounter;
    private int turelsCombatGroupId;
    private readonly List<Turel> turels = new ();
    private readonly Dictionary<int, int> turelToCombatId = new ();
    private readonly Dictionary<int, int> turelToProjectileGroupId = new ();
    private readonly List<ProjectileState> projectilesStateBuffer = new (64);

    private int rocketLauncherIdCounter;
    private readonly List<RocketLauncher> rocketLaunchers = new ();
    private readonly Dictionary<int, int> rocketLauncherToCombatId = new ();

    private int rocketIdCounter;
    private readonly Dictionary<int, List<Rocket>> rocketLauncherRocketsRegistry = new ();

    public WeaponController(WeaponView weaponView, ICombatService interactionService, TurelConfig turelConfig, IProjectileService projectileService, RocketLauncherConfig launcherConfig) {
        this.view = weaponView;
        this.combatService = interactionService;
        this.turelConfig = turelConfig;
        this.projectileService = projectileService;
        this.launcherConfig = launcherConfig;
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
            
            SpawnRocketLauncher(position + new Vector3(2, 0, 2), launcherConfig);
        }

    }

    public void Update() {
        UpdateRocketLandingCombat();
        FilterElapsedRockets();
        OperateRocketLaunchers();
        UpdateRocketLauncherView();

        UpdateProjectileHits();
        FilterDeadProjectiles();
        OperateTurels();
        UpdateTurelView();
    }

    private void SpawnRocketLauncher(Vector3 position, RocketLauncherConfig launcherConfig) {
        var launcherId = rocketLauncherIdCounter++;
        var rocketLauncher = new RocketLauncher(launcherId, position, launcherConfig);
        rocketLaunchers.Add(rocketLauncher);

        var rocketLauncherCombatId = combatService.RegisterAgent(position, turelsCombatGroupId);
        rocketLauncherToCombatId[launcherId] = rocketLauncherCombatId;

        rocketLauncherRocketsRegistry[launcherId] = new List<Rocket>();
        
        view.AddRocketLauncher(launcherId, position);
    }

    private void OperateRocketLaunchers() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var launcherCombatId = rocketLauncherToCombatId[rocketLauncher.Id];
            if (combatService.GetClosestEnemyAgentInRange(launcherCombatId, rocketLauncher.Radius, out var agentInfo, excludeGroup: turelsCombatGroupId)) {
                rocketLauncher.Aim(agentInfo.position);
            }
            
            if (rocketLauncher.Launch(Time.time, out var trajectory)) {
                SpawnRocket(rocketLauncher, trajectory);
            }
        }
    }

    private void SpawnRocket(RocketLauncher rocketLauncher, RocketTrajectory trajectory) {
        var nextRocketId = rocketIdCounter++;
        var rocket = new Rocket(nextRocketId, trajectory, Time.time, rocketLauncher.RocketFlyDuration);
        rocketLauncherRocketsRegistry[rocketLauncher.Id].Add(rocket);
        view.ShowRocketFly(rocketLauncher.Id, nextRocketId, trajectory, rocketLauncher.RocketFlyDuration);
    }

    private void UpdateRocketLandingCombat() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var launcherCombatId = rocketLauncherToCombatId[rocketLauncher.Id];
            
            foreach (var rocket in rocketLauncherRocketsRegistry[rocketLauncher.Id]) {
                if (rocket.ForwardLandingTime(Time.time)) {
                    combatService.ApplyExplosionDamage(launcherCombatId, rocket.Trajectory.landPoint, 3, rocketLauncher.RocketDamage);
                    var center = rocket.Trajectory.landPoint;
                    var color = Color.red;
                    var duration = 1f;
                    var radius = 3;
                    Debug.DrawLine(center, center + Vector3.right * radius, color, duration);
                    Debug.DrawLine(center, center + Vector3.left * radius, color, duration);
                    Debug.DrawLine(center, center + Vector3.forward * radius, color, duration);
                    Debug.DrawLine(center, center + Vector3.back * radius, color, duration);
                    view.ShowRocketExplosion(rocketLauncher.Id, rocket.Id);
                }
            }   
        }
    }

    private void FilterElapsedRockets() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var launcherRockets = rocketLauncherRocketsRegistry[rocketLauncher.Id];
            for (int i = 0; i < launcherRockets.Count; i++) {
                var rocket = launcherRockets[i];
                if (rocket.Landed) {
                    launcherRockets.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    private void UpdateRocketLauncherView() {
        foreach (var rocketLauncher in rocketLaunchers) {
            view.UpdateRocketLauncherOrientation(rocketLauncher.Id, rocketLauncher.AimPoint, rocketLauncher.RocketAmplitude);
        }
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