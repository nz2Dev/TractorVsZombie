using System;
using System.Collections.Generic;

using UnityEngine;

public class BodyController {
    
    private readonly BodyView view;
    private readonly PhysicsService physicsService;
    private readonly LocalAvoidanceService localAvoidanceService;

    private int idCounter;
    private Dictionary<int, BodyModel> registry = new ();

    public BodyController(PhysicsService physicsService, LocalAvoidanceService localAvoidanceService, BodyView view) {
        this.physicsService = physicsService;
        this.localAvoidanceService = localAvoidanceService;
        this.view = view;
    }

    public void Update() {
        UpdateMovements();
        SyncPositions();
    }

    public int SpawnBody(Vector3 position, BodyConfig config) {
        var nextId = ++idCounter;
        var model = new BodyModel(nextId, position, config);
        registry[model.Id] = model;
        model.AvoidanceId = localAvoidanceService.AddAgent(model.Position, model.AvoidanceConfig);
        model.PhysicsId = physicsService.RegisterPhysicsEntity(model.Position, model.PhysicsData.height, model.PhysicsData.radius);
        view.AddVisuals(model.Id, model.Position, model.VisualsPrefab);
        return nextId;
    }

    public void DeleteBody(int bodyId) {
        var model = registry[bodyId];
        localAvoidanceService.RemoveAgent(model.AvoidanceId);
        physicsService.UnregisterPhysicsEntity(model.PhysicsId);
        view.RemoveVisuals(model.Id);
        registry.Remove(bodyId);
    }

    public void Explode(int bodyId, Vector3 epicentr) {
        var model = registry[bodyId];
        if (!model.Grounded)
            return;

        model.Grounded = false;
        physicsService.UpdatePhysicsEntityPosition(model.PhysicsId, model.Position);
        physicsService.SetPhysicsActive(model.PhysicsId, true);
        physicsService.AddExplosionForce(model.PhysicsId, 10, epicentr, 4f, 1, ForceMode.Impulse);
    }

    public void Hit(int bodyId) {
        view.ShowTakeHit(bodyId);
    }

    public void Die(int bodyId, Vector3 deathSource, bool projectiled) {
        var model = registry[bodyId];
        model.Alive = false;
        if (projectiled) {
            view.ShowDeathByProjectile(model.Id, deathSource, blownAway: model.Grounded);
        } else {
            view.ShowDisolveDeath(model.Id);
        }
    }

    public void Move(int bodyId, Vector3 preferedVelocity) {
        var model = registry[bodyId];
        model.PreferedVelocity = preferedVelocity;
    }

    public void Attack(int bodyId) {
        view.ShowDirectFrontAttack(bodyId);
    }

    public BodyState ReadBodyState(int bodyId) {
        var model = registry[bodyId];
        return new BodyState {
            grounded = model.Grounded,
            position = model.Position,
            rotation = model.Rotation
        };
    }

    private void UpdateMovements() {
        foreach (var model in registry.Values) {
            var physicsPose = physicsService.GetEntityPose(model.PhysicsId);
            var keepFlying = !model.Grounded && physicsPose.InMotion;
            var becomeGrounded = !model.Grounded && !physicsPose.InMotion;
            var keepsGrouned = model.Grounded && !physicsPose.InMotion;
            
            if (keepFlying) {
                model.Position = physicsPose.Position;
                model.Rotation = physicsPose.Rotation;
            } else if (becomeGrounded) {
                model.Grounded = true;
                model.Position = physicsService.GetGroundPosition(model.Position);
                model.Rotation = model.Alive ? Quaternion.identity : model.Rotation;
                physicsService.SetPhysicsActive(model.PhysicsId, false);
            } else if (keepsGrouned && model.Alive) {
                localAvoidanceService.GetAgentPositionAndRotation(model.AvoidanceId, out var pos, out var rot);
                model.Position = pos;
                model.Rotation = rot;
            }
        }
    }

    private void SyncPositions() {
        foreach (var model in registry.Values) {
            localAvoidanceService.SetAgentPosition(model.AvoidanceId, model.Position);
            localAvoidanceService.SetPreferedVelocity(model.AvoidanceId, model.PreferedVelocity);
            view.UpdateTransform(model.Id, model.Position, model.Rotation);
        }
    }

}