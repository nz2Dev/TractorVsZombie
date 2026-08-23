using System;
using System.Collections.Generic;
using UnityEngine;

public class RagdollService {

    public struct RagdollPose {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public bool IsInteractive;
        public bool ContactWithGround;

        public RagdollPose(Vector3 position, Quaternion rotation, Vector3 velocity, bool isActive, bool contactWithGround) {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            IsInteractive = isActive;
            ContactWithGround = contactWithGround;
        }
    }

    private int idCounter;
    private readonly Dictionary<RagdollId, RagdollBody> bodyRegistry = new();
    private readonly Dictionary<int, RagdollObstacle> obstacleRegistry = new();
    
    public RagdollService() {
    }

    // this is implicitly also obstacle for vehicle?, raycasting? as it just adds collider into the scene, potentially interacting with those
    public int RegisterObstacle(Vector3 position, RagdollObstacle obstaclePrefab) {
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
    public RagdollId RegisterPhysicsEntity(Vector3 position, RagdollBody physicsBodyPrefab) {
        var entityId = new RagdollId(idCounter++);
        var physicsBodyInstance = GameObject.Instantiate(physicsBodyPrefab, position, Quaternion.identity);
        bodyRegistry[entityId] = physicsBodyInstance;
        return entityId;
    }

    public void UnregisterPhysicsEntity(RagdollId id) {
        if (bodyRegistry.TryGetValue(id, out var entity)) {
            GameObject.Destroy(entity.gameObject);
            bodyRegistry.Remove(id);
        }
    }

    public virtual void SetPhysicsActive(RagdollId id, bool active) {
        // make access consisten: either fail hard or play safe silently
        if (bodyRegistry.TryGetValue(id, out var entity)) {
            entity.SetDynamics(active);
        }
    }

    public virtual void AddExplosionForce(RagdollId id, float force, Vector3 position, float radius, float upwardsModifier = 0, ForceMode mode = ForceMode.Force) {
        var entity = bodyRegistry[id];
        entity.AddExplosionForce(force, position, radius, upwardsModifier, mode);
    }

    // consistency name, access: physics body 
    public void UpdatePhysicsEntityPosition(RagdollId id, Vector3 position) {
        if (bodyRegistry.TryGetValue(id, out var entity)) {
            entity.Teleport(position);
        }
    }

    public void UpdateEntityPose(RagdollId id, Vector3 position, Quaternion rotation) {
        if (bodyRegistry.TryGetValue(id, out var entity)) {
            entity.Set(position, rotation);
        }
    }

    public virtual RagdollPose GetEntityPose(RagdollId id) {
        if (bodyRegistry.TryGetValue(id, out var entity)) {
            return new RagdollPose(
                entity.Position,
                entity.Rotation,
                entity.LinearVelocity,
                isActive: entity.IsDynamic,
                entity.ContactWithGround
            );
        }
        return default;
    }

}
