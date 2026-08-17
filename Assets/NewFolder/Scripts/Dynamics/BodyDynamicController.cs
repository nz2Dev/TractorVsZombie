using System;

using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class BodyDynamicController {
    
    private readonly PhysicsService physicsService;

    private bool grounded = true;

    public BodyDynamicController(PhysicsService physicsService) {
        this.physicsService = physicsService;
    }

    public int Create() {
        return -1;
    }

    public BodyDynamicState GetState(int componentId) {
        return new BodyDynamicState {
            grounded = grounded
        };
    }

    public void Explode(int componentId, Explosion explosion) {
        physicsService.AddExplosionForce(-1, explosion.force, explosion.epicentr, explosion.radius, explosion.upwardModifier, ForceMode.Force);
    }

    public void Update() {
        var pose = physicsService.GetEntityPose(-1);
        if (grounded && pose.Velocity.sqrMagnitude > 0 && pose.IsDynamic) {
            grounded = false;
        }
    }
}