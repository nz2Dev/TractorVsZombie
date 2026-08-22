using System.Collections.Generic;
using System.ComponentModel;

using Combat;

using UnityEngine;

public class RocketController {

    private readonly RocketView view;
    private readonly CombatSystem combatSystem;
    private readonly RaycastService raycastService;
    private readonly InfantryController infantryController;

    private int idCounter = 0;
    private readonly Dictionary<int, RocketModel> registry = new ();

    public RocketController(RocketView view, CombatSystem combatSystem, InfantryController infantryController, RaycastService raycastService) {
        this.view = view;
        this.combatSystem = combatSystem;
        this.infantryController = infantryController;
        this.raycastService = raycastService;
    }

    public void Create(CombatId shooterCombatId, RocketPrototype prototype, FlyPath trajectory) {
        var shooterIsAlie = combatSystem.ReadState(shooterCombatId).alie;

        var nextRocketId = ++idCounter;
        var rocket = new RocketModel(nextRocketId, shooterCombatId, shooterIsAlie, Time.time, trajectory, prototype.config);
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
                view.ShowRocketExplosion(rocket.Id, rocket.Trajectory.landPoint, rocket.Config.explodeEffectClips);

                var targetFaction = !rocket.ShooterIsAlie;
                var targetRaycastLayer = CombatSystem.GetRaycastLayerForFaction(targetFaction);
                raycastService.Overlap(rocket.Trajectory.landPoint, rocket.Config.explosionRadius, 
                    targetRaycastLayer, out var overlappedRaycastIds);

                if (overlappedRaycastIds.Count > 0) {
                    infantryController.FindByRaycastIds(overlappedRaycastIds, out var overlappedInfantryIds);
                    
                    foreach (var nextInfantryId in overlappedInfantryIds) {    
                        infantryController.Explode(nextInfantryId, rocket.Trajectory.landPoint,
                            rocket.Config.explosionData);
                        
                        var nextInfantry = infantryController.GetInfantryState(nextInfantryId);
                        combatSystem.DealDamage(nextInfantry.combatId, new DamageInput {
                            damageSource = rocket.Trajectory.landPoint,
                            damageType = DamageType.Exposion,
                            damage = rocket.Config.damage,
                        });
                    }

                    // TODO: search for Armor/Platform/Truck entities
                }
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
