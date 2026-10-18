using System;
using System.Collections.Generic;
using System.ComponentModel;


using Interactions;

using UnityEngine;

public class RocketController {

    private readonly RocketView view;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly RaycastService raycastService;
    private readonly MotionSystem motionSystem;
    private readonly EntityMapping entityMapping;

    private int idCounter = 0;
    private readonly Dictionary<int, RocketModel> registry = new ();

    public RocketController(RocketView view, HealthSystem healthSystem, FactionRegistry factionRegistry, RaycastService raycastService, MotionSystem motionSystem, EntityMapping entityMapping) {
        this.view = view;
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.raycastService = raycastService;
        this.motionSystem = motionSystem;
        this.entityMapping = entityMapping;
    }

    public void Create(FactionId shooterFactionId, RocketPrototype prototype, FlyPath trajectory) {
        var shooterIsAlie = factionRegistry.ReadState(shooterFactionId).alie;

        var nextRocketId = ++idCounter;
        var rocket = new RocketModel(nextRocketId, shooterFactionId, shooterIsAlie, Time.time, trajectory, prototype.config);
        registry[nextRocketId] = rocket;

        view.ShowRocketFly(rocket.Id, prototype.visualsPrefab, rocket.LaunchTime,
            prototype.config.flyDuration, trajectory, prototype.config.flyShape, prototype.config.launchEffectClips);
    }

    public void Update() {
        UpdateRocketLanding();
        FilterElapsedRockets();
    }

    private void UpdateRocketLanding() {
        foreach (var rocket in registry.Values) {
            if (rocket.LaunchTime + rocket.Config.flyDuration < Time.time)
                rocket.Landed = true;

            if (rocket.Landed) {
                view.ShowRocketExplosion(rocket.Id, rocket.Trajectory.landPoint, rocket.Config.explodeEffectClips);

                var targetFaction = !rocket.ShooterIsAlie;
                var targetRaycastLayer = FactionRegistry.GetRaycastLayerForFaction(targetFaction);
                raycastService.Overlap(rocket.Trajectory.landPoint, rocket.Config.explosionRadius, 
                    targetRaycastLayer, out var overlappedRaycastIds);

                if (overlappedRaycastIds.Count > 0) {
                    entityMapping.FindByRaycastIds(overlappedRaycastIds, out var overlappedComponents);
                    
                    foreach (var nextComponents in overlappedComponents) {   
                        if (nextComponents.motionId.HasValue) {
                            var explosionConfig = rocket.Config.explosionData;

                            var forceVariation = explosionConfig.force * rocket.Config.forceRangeVariation;
                            explosionConfig.force += UnityEngine.Random.Range(-1, 1) * forceVariation;

                            motionSystem.AddExplosionEffect(nextComponents.motionId.Value, new Explosion {
                                epicentr = rocket.Trajectory.landPoint,
                                config = explosionConfig
                            });
                        } 
                        
                        if (nextComponents.healthId.HasValue) {
                            healthSystem.DealDamage(nextComponents.healthId.Value, new DamageInput {
                                damageSource = rocket.Trajectory.landPoint,
                                damageType = DamageType.Exposion,
                                damage = rocket.Config.damage,
                            });
                        }
                    }
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
