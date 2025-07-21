using System;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsService {
    private readonly Dictionary<int, PhysicsEntity> entities = new();
    private readonly Dictionary<Collider, int> colliderToId = new();
    private readonly Transform container;
    private const int MaxHits = 32;
    private readonly Collider[] hitBuffer = new Collider[MaxHits];
    private readonly List<int> sphereQueryResult = new List<int>(64);

    public PhysicsService(Transform container = null) {
        this.container = container;
    }

    internal class PhysicsEntity {
        public int Id { get; }
        public GameObject GameObject { get; }
        public CapsuleCollider Collider { get; }
        public Rigidbody Rigidbody { get; }

        public PhysicsEntity(int id, GameObject go, CapsuleCollider collider, Rigidbody rb) {
            Id = id;
            GameObject = go;
            Collider = collider;
            Rigidbody = rb;
        }
    }

    public struct PhysicsEntityPose {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;

        public PhysicsEntityPose(Vector3 position, Quaternion rotation, Vector3 velocity) {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
        }
    }

    public int RegisterPhysicsEntity(int id, Vector3 position, float height, float radius) {
        if (entities.ContainsKey(id))
            throw new ArgumentException($"Physics entity with id {id} already exists.");
        var go = new GameObject($"Physics Entity {id} (New)", typeof(CapsuleCollider), typeof(Rigidbody));
        go.transform.SetParent(container, false);
        go.transform.position = position;
        var capsule = go.GetComponent<CapsuleCollider>();
        capsule.isTrigger = true;
        capsule.height = height;
        capsule.radius = radius;
        capsule.direction = 1;
        capsule.center = new Vector3(0f, height * 0.5f, 0f);
        var rb = go.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        entities[id] = new PhysicsEntity(id, go, capsule, rb);
        colliderToId[capsule] = id;
        return id;
    }

    public void SetPhysicsActive(int id, bool active) {
        if (entities.TryGetValue(id, out var entity)) {
            entity.Rigidbody.isKinematic = !active;
        }
    }

    public void AddExplosionForce(int id, float force, Vector3 position, float radius, float upwardsModifier = 0, ForceMode mode = ForceMode.Force) {
        if (entities.TryGetValue(id, out var entity)) {
            entity.Rigidbody.AddExplosionForce(force, position, radius, upwardsModifier, mode);
        }
    }

    public void UpdatePhysicsEntityShape(int id, float height, float radius) {
        if (entities.TryGetValue(id, out var entity)) {
            entity.Collider.height = height;
            entity.Collider.radius = radius;
            entity.Collider.direction = 1;
            entity.Collider.center = new Vector3(0f, height * 0.5f, 0f);
        }
    }

    public void UpdatePhysicsEntityPosition(int id, Vector3 position) {
        if (entities.TryGetValue(id, out var entity)) {
            entity.GameObject.transform.position = position;
        }
    }

    public IReadOnlyList<int> QuerySphere(Vector3 center, float radius) {
        sphereQueryResult.Clear();
        int hitCount = Physics.OverlapSphereNonAlloc(center, radius, hitBuffer, ~0, QueryTriggerInteraction.Collide);
        for (int i = 0; i < hitCount; i++) {
            if (colliderToId.TryGetValue(hitBuffer[i], out var id)) {
                sphereQueryResult.Add(id);
            }
        }
        return sphereQueryResult;
    }

    public void UnregisterPhysicsEntity(int id) {
        if (entities.TryGetValue(id, out var entity)) {
            colliderToId.Remove(entity.Collider);
            UnityEngine.Object.Destroy(entity.GameObject);
            entities.Remove(id);
        }
    }

    public PhysicsEntityPose GetEntityPose(int id) {
        if (entities.TryGetValue(id, out var entity)) {
            var rb = entity.Rigidbody;
            return new PhysicsEntityPose(
                rb.position,
                rb.rotation,
                rb.velocity
            );
        }
        return default;
    }
}
