
using System;
using System.Collections.Generic;

using UnityEngine;

public class WeaponController {
    
    private readonly WeaponView view;
    private readonly RocketController rocketController;
    private readonly ProjectileController projectileController;

    private int idCounter;
    private Dictionary<int, WeaponModel> registry = new ();

    public WeaponController(WeaponView view, RocketController rocketController, ProjectileController projectileController) {
        this.view = view;
        this.rocketController = rocketController;
        this.projectileController = projectileController;
    }

    public void Update() {
        UpdateFire();
    }

    public int SpawnWeapon(int ownerCombatId, WeaponPrototype prototype) {
        var nextId = ++idCounter;
        var model = new WeaponModel(nextId, ownerCombatId, prototype.position, prototype.config);
        registry[nextId] = model;
        view.AddWeapon(model.Id, prototype.position, prototype.visualsPrefab);
        return model.Id;
    }

    public void AimWeapon(int weaponId, Vector3 target) {
        var weapon = registry[weaponId];
        var aimInput = new AimInput { deltaTime = Time.deltaTime, position = weapon.Position, previousAim = weapon.AimPoint, targetAim = target };
        weapon.AimPoint = AimStrategy.Evaluate(weapon.AimConfig, aimInput);
        view.UpdateAim(weapon.Id, weapon.AimPoint, weapon.BallisticConfig);
    }

    public void MoveWeapon(int weaponId, Vector3 position) {
        var weapon = registry[weaponId];
        weapon.Position = position;
        view.UpdatePosition(weaponId, weapon.Position);
    }

    public void DeleteWeapon(int weaponId) {
        view.RemoveWeapon(weaponId);
        registry.Remove(weaponId);
    }

    private void UpdateFire() {
        foreach (var model in registry.Values) {
            if (model.LastShootTime + model.CooldownSec < Time.time) {
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
        var bulletVelocity = (weapon.AimPoint - weapon.LaunchPoint).normalized * weapon.BallisticConfig.bullet.speed;
        projectileController.SpawnBulletProjectile(
            weapon.CombatId, 
            new Bullet { firePoint = weapon.LaunchPoint, velocity = bulletVelocity},
            weapon.BallisticConfig.bullet
        );
    }

    private void FireRocket(WeaponModel weapon) {
        rocketController.SpawnRocket(
            weapon.CombatId,
            new RocketTrajectory {
                launchPoint = weapon.LaunchPoint,
                landPoint = weapon.AimPoint,
            },
            weapon.BallisticConfig.rocket
        );
    }

}