using UnityEngine;

public class PhysicsManager : MonoBehaviour {

    public PhysicsBody InstantiateBody(PhysicsBody bodyPrefab, Vector3 position, Quaternion rotation) {
        var instance = GameObject.Instantiate(bodyPrefab, position, rotation);
        return instance;
    }
    
    internal void DestroyBody(PhysicsBody body) {
        UnityEngine.Object.Destroy(body.gameObject);
    }

    internal PhysicsObstacleNew InstantiateObstacle(PhysicsObstacleNew obstaclePrefab, Vector3 position, Quaternion rotation) {
        var instance = GameObject.Instantiate(obstaclePrefab, position, rotation);
        return instance;
    }

    internal void DestroyObstacle(PhysicsObstacleNew obstacle) {
        UnityEngine.Object.Destroy(obstacle.gameObject);
    }
    
}