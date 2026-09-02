using System.Collections.Generic;

using UnityEngine;

public class CollisionService {
    
    private readonly CollisionServiceConfig config;

    private readonly Dictionary<CollisionObstacleId, CollisionObstacle> obstacleRegistry = new();
    private int idCounter;
    
    public CollisionService(CollisionServiceConfig config) {
        this.config = config;
    }

    // this is implicitly also obstacle for vehicle?, raycasting? as it just adds collider into the scene, potentially interacting with those
    public CollisionObstacleId RegisterObstacle(Vector3 position, CollisionObstacle obstaclePrefab) {
        var id = new CollisionObstacleId(++idCounter);
        var obstacleInstance = GameObject.Instantiate(obstaclePrefab, position, Quaternion.identity);
        obstacleRegistry[id] = obstacleInstance;
        return id;
    }

    public void UnregisterObstacle(CollisionObstacleId id) {
        if (obstacleRegistry.TryGetValue(id, out var obstacle)) {
            GameObject.Destroy(obstacle.gameObject);
            obstacleRegistry.Remove(id);
        }
    }

    public Vector3 GetClosestVerticalGroundPoint(Vector3 position) {
        if (Physics.Raycast(new Ray(position + Vector3.up, Vector3.down), out var hitInfo, maxDistance: 100, config.groundMask)) {
            return hitInfo.point;
        } else {
            return Vector3.zero;
        }
    }

    public Vector3 GetGroundHitPosition(Ray ray) {
        if (Physics.Raycast(ray, out var hitInfo, maxDistance: 1000, config.groundMask)) {
            return hitInfo.point;
        } else {
            return Vector3.zero;
        }
    }

    public bool RaycastEnvironment(Ray ray, float maxDistance, out RaycastHit hitInfo) {
        return Physics.Raycast(ray, out hitInfo,  maxDistance, config.environmentMask);
    }
}