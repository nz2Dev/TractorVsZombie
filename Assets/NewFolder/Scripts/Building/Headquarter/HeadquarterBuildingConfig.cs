using UnityEngine;

[CreateAssetMenu(fileName = "HeadquarterBuildingConfig", menuName = "HeadquarterBuildingConfig", order = 0)]
public class HeadquarterBuildingConfig : ScriptableObject {
    public int radius = 3;
    public int maxHealth = 100;
    public bool alie = false;
    public GameObject visualsPrefab;
    public PhysicsObstacle vehicleObstaclePrefab;
    public PhysicsObstacleNew physicsObstaclePrefab;
    public PhysicsObstacle avoidanceObstaclePrefab;
}