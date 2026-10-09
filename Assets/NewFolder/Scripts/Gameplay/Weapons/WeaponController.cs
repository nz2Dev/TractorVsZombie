using System.Collections.Generic;

using Combat;

using UnityEngine;

public class WeaponController {
    
    private readonly WeaponView view;
    private readonly RocketController rocketController;
    private readonly ProjectileController projectileController;

    private int idCounter;
    private readonly Dictionary<int, WeaponModel> registry = new ();

    public WeaponController(WeaponView view, RocketController rocketController, ProjectileController projectileController) {
        this.view = view;
        this.rocketController = rocketController;
        this.projectileController = projectileController;
    }

    public void Update() {
        UpdateFire();
    }

    public int SpawnWeapon(CombatId ownerCombatId, WeaponPrototype prototype) {
        var nextId = ++idCounter;
        var model = new WeaponModel(nextId, ownerCombatId, prototype.config);
        registry[nextId] = model;

        model.Position = prototype.position;
        model.BallisticPrototype = prototype.ballisticPrototype;
        model.BallisticLaunchOffset = prototype.ballisticLaunchOffset;
        model.LastShootTime = Time.time;

        view.AddWeapon(model.Id, prototype.position, prototype.visualsPrefab);
        return model.Id;
    }

    public WeaponState ReadWeaponState(int weaponId) {
        var weapon = registry[weaponId];
        return new WeaponState {
            aimConfig = weapon.Config.aimConfig
        };
    }

    public void AimWeapon(int weaponId, Vector3 target) {
        var weapon = registry[weaponId];
        var aimInput = new AimInput { deltaTime = Time.deltaTime, position = weapon.Position, previousAim = weapon.AimPoint, targetAim = target };
        weapon.AimPoint = AimStrategy.Evaluate(weapon.Config.aimConfig, aimInput);
        view.UpdateAim(weapon.Id, weapon.AimPoint, weapon.BallisticPrototype);
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
            if (model.LastShootTime + model.Config.cooldownSec < Time.time) {
                model.LastShootTime = Time.time;
                FireBallistic(model);
                view.ShowActivation(model.Id, model.BallisticPrototype);
            }
        }
    }

    private void FireBallistic(WeaponModel weapon) {
        switch (weapon.BallisticPrototype.type) {
            case BallisticType.Bullet:
                FireBullet(weapon);
                break;
            case BallisticType.Rocket:
                FireRocket(weapon);
                break;
        }
    }

    private void FireBullet(WeaponModel weapon) {
        var launchPoint = weapon.Position + weapon.BallisticLaunchOffset;
        var projectileDirection = (weapon.AimPoint - launchPoint).normalized;
        projectileController.Create(
            weapon.CombatId,
            weapon.BallisticPrototype.projectilePrototype,
            new Orientation { 
                origin = launchPoint, 
                direction = projectileDirection
            }
        );
    }

    private void FireRocket(WeaponModel weapon) {
        rocketController.Create(
            weapon.CombatId,
            weapon.BallisticPrototype.rocketPrototype,
            new FlyPath {
                launchPoint = weapon.Position + weapon.BallisticLaunchOffset,
                landPoint = weapon.AimPoint,
            }
        );
    }

}