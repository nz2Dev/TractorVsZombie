using System.Collections.Generic;

using UnityEngine;

public class BodySimulator {
    
    private readonly PhysicsService physicsService;

    private int idCounter;
    private Dictionary<int, BodyModel> registry = new ();

    public BodySimulator(PhysicsService physicsService) {
        this.physicsService = physicsService;
    }

    public void Update() {
        UpdateMovements();
    }

    public int SpawnBody(Vector3 position, BodyConfig config) {
        var nextId = ++idCounter;
        var model = new BodyModel(nextId, position, config);
        model.PhysicsId = physicsService.RegisterPhysicsEntity(model.Position, model.PhysicsData.height, model.PhysicsData.radius);        
        registry[model.Id] = model;
        return nextId;
    }

    public void DeleteBody(int bodyId) {
        var model = registry[bodyId];
        physicsService.UnregisterPhysicsEntity(model.PhysicsId);
        registry.Remove(bodyId);
    }

    public void ApplyImpulse(int bodyId, Vector3 epicentr) {
        var model = registry[bodyId];
        model.Grounded = false;
        physicsService.UpdatePhysicsEntityPosition(model.PhysicsId, model.Position);
        physicsService.SetPhysicsActive(model.PhysicsId, true);
        physicsService.AddExplosionForce(model.PhysicsId, 10, epicentr, 4f, 1, ForceMode.Impulse);
    }

    public void DisableRecovery(int bodyId) {
        var model = registry[bodyId];
        model.CanRecover = false;   
    }

    public void ApplyMovement(int bodyId, Vector3 velocity) {
        if (registry.TryGetValue(bodyId, out var model)) {
            model.DrivenVelocity = velocity;
        }
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
                model.Rotation = model.CanRecover ? Quaternion.identity : model.Rotation;
                physicsService.SetPhysicsActive(model.PhysicsId, false);
            } else if (keepsGrouned && model.CanRecover) {
                model.Position = model.Position += model.DrivenVelocity * Time.deltaTime;
                model.Rotation = Quaternion.LookRotation(model.DrivenVelocity.normalized, Vector3.up);
            }
        }
    }

}