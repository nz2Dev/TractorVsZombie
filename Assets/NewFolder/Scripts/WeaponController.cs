
using System;
using System.Collections.Generic;

using UnityEngine;

public class WeaponController {
    
    private readonly WeaponView view;
    private readonly RocketController rocketController;
    private readonly ProjectileController projectileController;
    private readonly CombatService combatService;

    private int idCounter;
    private Dictionary<int, WeaponModel> registry = new ();

    public void Update() {
        OperateWeapons();
    }

    public int SpawnWeapon(int combatId, Vector3 position, WeaponConfig config) {
        var nextId = ++idCounter;
        var model = new WeaponModel(nextId, combatId, position, config);
        registry[nextId] = model;
        view.AddWeapon(model.Id, position, model.VisualsPrefab);
        return model.Id;
    }

    public void MoveWeapon(int weaponId, Vector3 position) {
        var weapon = registry[weaponId];
        weapon.Position = position;
        view.UpdatePosition(weaponId, weapon.Position);
    }

    private void OperateWeapons() {
        foreach (var model in registry.Values) {
            var enemySearchRadius = model.AimConfig.range;
            
            if (combatService.GetClosestEnemyAgentInRange(model.CombatId, enemySearchRadius, out var agentInfo)) {
                var aimInput = new AimInput { deltaTime = Time.deltaTime, position = model.Position, previousAim = model.AimPoint, targetAim = agentInfo.position };
                model.AimPoint = AimStrategy.Evaluate(model.AimConfig, aimInput);
                view.UpdateAim(model.Id, model.AimPoint, model.BallisticConfig);
            }
            
            if (Time.time + model.CooldownSec > Time.time) {
                model.LastShootTime = Time.time;
                FireBallistic(model);
                view.ShowActivation(model.Id, model.BallisticConfig.type);
            }
        }
    }

    private void FireBallistic(WeaponModel weapon) {
        switch (weapon.BallisticConfig.type) {
            case BallisticType.Bullet:
                FireBullet(weapon);
                break;
            case BallisticType.Rocket:
                FireRocket(weapon);
                break;
        }
    }

    private void FireBullet(WeaponModel weapon) {
        var bulletVelocity = (weapon.AimPoint - weapon.LaunchPoint).normalized * weapon.BallisticConfig.bulletSpeed;
        projectileController.SpawnBulletProjectile(
            weapon.CombatId, 
            new Bullet { firePoint = weapon.LaunchPoint, velocity = bulletVelocity},
            weapon.BallisticConfig.bulletShootAudioClips
        );
    }

    private void FireRocket(WeaponModel weapon) {
        rocketController.SpawnRocket(
            weapon.CombatId,
            new RocketTrajectory {
                launchPoint = weapon.LaunchPoint,
                landPoint = weapon.AimPoint,
                flyDuration = weapon.BallisticConfig.rocketFlyDuration,
                height = weapon.BallisticConfig.rocketAmplitude,
                launchEffectClips = weapon.BallisticConfig.launchEffectClips,
                explodeEffectClips = weapon.BallisticConfig.explodeEffectClips,
            }
        );
    }

}