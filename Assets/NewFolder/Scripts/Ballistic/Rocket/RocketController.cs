using System.Collections.Generic;

using UnityEngine;

public class RocketController {
    
    private readonly RocketView view;
    private readonly CombatSystem combatSystem;

    private int idCounter = 0;
    private readonly Dictionary<int, RocketModel> registry = new ();

    public RocketController(RocketView view, CombatSystem combatSystem) {
        this.view = view;
        this.combatSystem = combatSystem;
    }

    public void Create(int shooterId, RocketPrototype prototype, FlyPath trajectory) {
        var nextRocketId = ++idCounter;
        var rocket = new RocketModel(nextRocketId, shooterId, Time.time, trajectory, prototype.config);
        registry[nextRocketId] = rocket;
        
        view.ShowRocketFly(rocket.Id, prototype.visualsPrefab, rocket.LaunchTime, 
            prototype.config.flyDuration, trajectory, prototype.config.flyShape, prototype.config.launchEffectClips);
    }

    public void Update() {
        UpdateRocketLandingCombat();
        FilterElapsedRockets();
    }

    private void UpdateRocketLandingCombat() {
        foreach (var rocket in registry.Values) {
            if (rocket.LaunchTime + rocket.Config.flyDuration < Time.time)
                rocket.Landed = true;

            if (rocket.Landed) {
                combatSystem.ApplyExplosionDamage(rocket.ShooterId, rocket.Trajectory.landPoint, 
                    rocket.Config.explosionRadius, rocket.Config.damage, rocket.Config.explosionData);
                view.ShowRocketExplosion(rocket.Id, rocket.Trajectory.landPoint, rocket.Config.explodeEffectClips);
            }
        }   
    }

    private void FilterElapsedRockets() {
        var landedRockets = new List<int>();
        foreach (var rocket in registry.Values) {
            if (rocket.Landed) {
                landedRockets.Add(rocket.Id);
            }
        }
        foreach (var landedRocketId in landedRockets) {
            registry.Remove(landedRocketId);
        }
    }
}