using UnityEngine;

[CreateAssetMenu(fileName = "HeadquarterBuildingConfig", menuName = "HeadquarterBuildingConfig", order = 0)]
public class HeadquarterBuildingConfig : ScriptableObject {
    public GameObject visualsPrefab; // move to prototype
    public PhysicsObstacle vehicleObstaclePrefab; // prototype component
    public RagdollObstacle physicsObstaclePrefab; // prototpye component
    public PhysicsObstacle avoidanceObstaclePrefab; // prototpe component
}