using System;
using System.Collections.Generic;
using UnityEngine;

public class RagdollService {

    public struct RagdollPose {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public bool IsInteractive;
        public bool InMotion;
        public bool Pending;

        public RagdollPose(Vector3 position, Quaternion rotation, Vector3 velocity, bool inMotion, bool pending, bool isActive) {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            InMotion = inMotion;
            Pending = pending;
            IsInteractive = isActive;
        }
    }

    private int idCounter;
    private readonly Dictionary<int, RagdollBody> bodyRegistry = new();
    private readonly Dictionary<int, PhysicsObstacleNew> obstacleRegistry = new();
    
    public RagdollService() {
    }

    public int RegisterObstacle(Vector3 position, PhysicsObstacleNew obstaclePrefab) {
        var id = ++idCounter;
        var obstacleInstance = GameObject.Instantiate(obstaclePrefab, position, Quaternion.identity);
        obstacleRegistry[id] = obstacleInstance;
        return id;
    }

    public void UnregisterObstacle(int id) {
        if (obstacleRegistry.TryGetValue(id, out var obstacle)) {
            GameObject.Destroy(obstacle.gameObject);
            obstacleRegistry.Remove(id);
        }
    }

    // register physics body
    public int RegisterPhysicsEntity(Vector3 position, RagdollBody physicsBodyPrefab) {
        var entityId = idCounter++;
        var physicsBodyInstance = GameObject.Instantiate(physicsBodyPrefab, position, Quaternion.identity);
        bodyRegistry[entityId] = physicsBodyInstance;
        return entityId;
    }

    public void UnregisterPhysicsEntity(int id) {
        if (bodyRegistry.TryGetValue(id, out var entity)) {
            GameObject.Destroy(entity.gameObject);
            bodyRegistry.Remove(id);
        }
    }

    public virtual void SetPhysicsActive(int id, bool active) {
        // make access consisten: either fail hard or play safe silently
        if (bodyRegistry.TryGetValue(id, out var entity)) {
            entity.SetDynamics(active);
        }
    }

    public virtual void AddExplosionForce(int id, float force, Vector3 position, float radius, float upwardsModifier = 0, ForceMode mode = ForceMode.Force) {
        var entity = bodyRegistry[id];
        entity.AddExplosionForce(force, position, radius, upwardsModifier, mode);
    }

    // consistency name, access: physics body 
    public void UpdatePhysicsEntityPosition(int id, Vector3 position) {
        if (bodyRegistry.TryGetValue(id, out var entity)) {
            entity.Teleport(position);
        }
    }

    public void UpdateEntityPose(int id, Vector3 position, Quaternion rotation) {
        if (bodyRegistry.TryGetValue(id, out var entity)) {
            entity.Set(position, rotation);
        }
    }

    public virtual RagdollPose GetEntityPose(int id) {
        if (bodyRegistry.TryGetValue(id, out var entity)) {
            const float minFlyTime = 0.5f;
            // domain logic here, keep this closer to usage, e.g in Infantry
            var isInMotion = entity.ExplosionTime + minFlyTime > Time.time || entity.LinearVelocity.sqrMagnitude > 0.75f;
            return new RagdollPose(
                entity.Position,
                entity.Rotation,
                entity.LinearVelocity,
                inMotion: isInMotion,
                pending: entity.IsDynamic,
                isActive: entity.IsDynamic
            );
        }
        return default;
    }

}
