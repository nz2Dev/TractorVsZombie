using System.Collections.Generic;

using UnityEngine;

public class RocketController {
    
    private readonly RocketView view;
    private readonly SoundManager soundManager;
    private readonly CombatService combatService;

    private int idCounter = 0;
    private Dictionary<int, RocketModel> models = new ();

    public RocketController(RocketView view, SoundManager soundManager, CombatService combatService) {
        this.view = view;
        this.soundManager = soundManager;
        this.combatService = combatService;
    }

    public void SpawnRocket(int shooterId, RocketTrajectory trajectory) {
        var nextRocketId = ++idCounter;
        var rocket = new RocketModel(nextRocketId, shooterId, Time.time, trajectory);
        models[nextRocketId] = rocket;
        view.ShowRocketFly(rocket.Id, rocket.LaunchTime, trajectory);
        soundManager.PlayEffect(trajectory.launchPoint, trajectory.launchEffectClips);
    }

    public void Update() {
        UpdateRocketLandingCombat();
        FilterElapsedRockets();
    }

    private void UpdateRocketLandingCombat() {
        foreach (var rocket in models.Values) {
            if (rocket.ForwardLandingTime(Time.time)) {
                combatService.ApplyExplosionDamage(rocket.ShooterId, rocket.Trajectory.landPoint, 3, 1);
                view.ShowRocketExplosion(rocket.Id);
                soundManager.PlayEffect(rocket.Trajectory.landPoint, rocket.Trajectory.explodeEffectClips);
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