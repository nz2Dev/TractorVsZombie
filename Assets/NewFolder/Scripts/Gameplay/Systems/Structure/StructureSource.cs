using System;
using UnityEngine;

[Serializable]
public struct StructureSource {
    [NonNull, Local, SerializeField] private ORCAObstacleVertices avoidanceObstaclePrefab;
    [NonNull, Local, SerializeField] private CollisionObstacle collisionObstaclePrefab;

    public StructurePrototype GetPrototype(Vector3 position) => new() {
        position = position,
        avoidanceObstaclePrefab = avoidanceObstaclePrefab,
        collisionObstaclePrefab = collisionObstaclePrefab
    };
}
