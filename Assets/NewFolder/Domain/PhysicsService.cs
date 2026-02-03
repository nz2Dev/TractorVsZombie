using System;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsService {

    private readonly int operationalLayer;
    private readonly int obstacleLayer;
    private readonly Dictionary<int, PhysicsEntity> entities = new();
    private readonly Dictionary<int, GameObject> obstacleRegistry = new();
    private readonly Dictionary<Collider, int> colliderToId = new();
    private readonly Transform container;
    private const int MaxHits = 32;
    private readonly Collider[] hitBuffer = new Collider[MaxHits];
    private readonly List<int> sphereQueryResult = new List<int>(64);
    private int idCounter;

    public PhysicsService(Transform container = null, int operationalLayer = 0, int obstacleLayer = 0) {
        this.container = container;
        this.operationalLayer = operationalLayer;
        this.obstacleLayer = obstacleLayer;
    }

    public int RegisterObstacle(Vector3 position, Vector3 size) {
        var id = ++idCounter;
        var go = new GameObject($"PhysicsObstacle_{id}");
        go.layer = obstacleLayer;
        go.transform.SetParent(container, false);
        go.transform.position = position;
        var collider = go.AddComponent<BoxCollider>();
        collider.size = size;
        obstacleRegistry[id] = go;
        return id;
    }

    public void UnregisterObstacle(int id) {
        if (obstacleRegistry.TryGetValue(id, out var go)) {
            GameObject.Destroy(go);
            obstacleRegistry.Remove(id);
        }
    }

    internal class PhysicsEntity {
        public int Id { get; }
        public GameObject GameObject { get; }
        public CapsuleCollider Collider { get; }
        public Rigidbody Rigidbody { get; }
        public float ExplosionTime { get; set; } = float.NegativeInfinity;

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
        public bool InMotion;
        public bool Pending;

        public PhysicsEntityPose(Vector3 position, Quaternion rotation, Vector3 velocity, bool inMotion, bool pending) {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            InMotion = inMotion;
            Pending = pending;
        }
    }

    public int RegisterPhysicsEntity(Vector3 position, float height, float radius) {
        var entityId = idCounter++;
        var go = new GameObject($"Physics Entity {entityId} (New)", typeof(CapsuleCollider), typeof(Rigidbody));
        go.layer = operationalLayer;
        go.transform.SetParent(container, false);
        go.transform.position = position;
        var capsule = go.GetComponent<CapsuleCollider>();
        capsule.isTrigger = true;
        capsule.height = height;
        capsule.radius = Mathf.Min(radius, height * 0.5f);
        capsule.direction = 1;
        capsule.center = new Vector3(0f, height * 0.5f, 0f);
        var rb = go.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        entities[entityId] = new PhysicsEntity(entityId, go, capsule, rb);
        colliderToId[capsule] = entityId;
        return entityId;
    }

    public void SetPhysicsActive(int id, bool active) {
        if (entities.TryGetValue(id, out var entity)) {
            entity.Rigidbody.isKinematic = !active;
            entity.Rigidbody.useGravity = active;
            entity.Collider.isTrigger = !active;
        }
    }

    public void AddExplosionForce(int id, float force, Vector3 position, float radius, float upwardsModifier = 0, ForceMode mode = ForceMode.Force) {
        var entity = entities[id];
        entity.Rigidbody.AddExplosionForce(force, position, radius, upwardsModifier, mode);
        entity.ExplosionTime = Time.time;
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
            entity.Rigidbody.position = position;
        }
    }

    public IReadOnlyList<int> QuerySphere(Vector3 center, float radius) {
        sphereQueryResult.Clear();
        int hitCount = Physics.OverlapSphereNonAlloc(center, radius, hitBuffer, 1 << operationalLayer, QueryTriggerInteraction.Collide);
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

    public Vector3 GetGroundPosition(Vector3 position) {
        var groundLayer = LayerMask.NameToLayer("Default"); // TODO use input variable;
        if (Physics.Raycast(position + Vector3.up, Vector3.down, out var hitInfo, 100, 1 << groundLayer)) {
            return hitInfo.point;
        } else {
            return Vector3.zero;
        }
    }

    public Vector3 GetGroundHitPosition(Ray ray) {
        var groundLayer = LayerMask.NameToLayer("Default"); // TODO use input variable;
        if (Physics.Raycast(ray, out var hitInfo, 100, 1 << groundLayer)) {
            return hitInfo.point;
        } else {
            return Vector3.zero;
        }
    }

    public PhysicsEntityPose GetEntityPose(int id) {
        if (entities.TryGetValue(id, out var entity)) {
            var rb = entity.Rigidbody;
            const float minFlyTime = 0.5f;
            var isInMotion = entity.ExplosionTime + minFlyTime > Time.time || rb.linearVelocity.sqrMagnitude > 0.75f;
            return new PhysicsEntityPose(
                rb.position,
                rb.rotation,
                rb.linearVelocity,
                inMotion: isInMotion,
                pending: !rb.isKinematic
            );
        }
        return default;
    }
}
