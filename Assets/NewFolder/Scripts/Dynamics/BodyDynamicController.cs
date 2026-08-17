using System;

using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class BodyDynamicController {
    
    private readonly PhysicsService physicsService;

    private bool grounded = true;
    private BodyDynamicConfig config;

    public BodyDynamicController(PhysicsService physicsService) {
        this.physicsService = physicsService;
    }

    public int Create() {
        return Create(new BodyDynamicPrototype {
            config = new BodyDynamicConfig {
                stopSpeedLimit = 0.1f
            }
        });
    }

    public int Create(BodyDynamicPrototype prototype) {
        config = prototype.config;
        return -1;
    }

    public BodyDynamicState GetState(int componentId) {
        return new BodyDynamicState {
            grounded = grounded
        };
    }

    public void Explode(int componentId, Explosion explosion) {
        physicsService.SetPhysicsActive(-1, true);
        physicsService.AddExplosionForce(-1, explosion.force, explosion.epicentr, explosion.radius, explosion.upwardModifier, ForceMode.Force);
    }

    public void Update() {
        var pose = physicsService.GetEntityPose(-1);
        var limitSqared = config.stopSpeedLimit * config.stopSpeedLimit;
        var inMotion = pose.Velocity.sqrMagnitude > limitSqared;
        if (inMotion) {
            if (grounded) {
                grounded = false;
            }
        } else {
            if (!grounded || pose.IsInteractive) {
                physicsService.SetPhysicsActive(-1, false);
            }
        }
    }
}