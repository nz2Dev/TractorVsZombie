using System.Collections.Generic;

using UnityEngine;

public class RocketController {
    
    private readonly RocketView view;
    private readonly SoundManager soundManager;
    private readonly CombatSystem combatSystem;

    private int idCounter = 0;
    private Dictionary<int, RocketModel> models = new ();

    public RocketController(RocketView view, SoundManager soundManager, CombatSystem combatSystem) {
        this.view = view;
        this.soundManager = soundManager;
        this.combatSystem = combatSystem;
    }

    public void SpawnRocket(int shooterId, RocketTrajectory trajectory, RocketConfig config) {
        var nextRocketId = ++idCounter;
        var rocket = new RocketModel(nextRocketId, shooterId, Time.time, trajectory, config);
        models[nextRocketId] = rocket;
        view.ShowRocketFly(rocket.Id, rocket.LaunchTime, trajectory, config);
        soundManager.PlayEffect(trajectory.launchPoint, config.launchEffectClips);
    }

    public void Update() {
        UpdateRocketLandingCombat();
        FilterElapsedRockets();
    }

    private void UpdateRocketLandingCombat() {
        foreach (var rocket in models.Values) {
            if (rocket.ForwardLandingTime(Time.time)) {
                combatSystem.ApplyExplosionDamage(rocket.ShooterId, rocket.Trajectory.landPoint, rocket.Config.damage, 1);
                view.ShowRocketExplosion(rocket.Id);
                soundManager.PlayEffect(rocket.Trajectory.landPoint, rocket.Config.explodeEffectClips);
            }
        }   
    }

    private void FilterElapsedRockets() {
        var landedRockets = new List<int>();
        foreach (var rocket in models.Values) {
            if (rocket.Landed) {
                landedRockets.Add(rocket.Id);
            }
        }
        foreach (var landedRocketId in landedRockets) {
            models.Remove(landedRocketId);
        }
    }
}