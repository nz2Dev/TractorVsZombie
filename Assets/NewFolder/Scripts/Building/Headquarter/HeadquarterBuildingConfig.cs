using UnityEngine;

[CreateAssetMenu(fileName = "HeadquarterBuildingConfig", menuName = "HeadquarterBuildingConfig", order = 0)]
public class HeadquarterBuildingConfig : ScriptableObject {
    public int radius = 3;
    public int maxHealth = 100;
    public bool alie = false;
    public GameObject visualsPrefab;
    public GameObject vehicleObstaclePrefab;
    public Vector3 physicsObstacleSize = new Vector3(3, 2, 3);
    public Vector3 VehicleObstacleSize => vehicleObstaclePrefab.GetComponent<BoxCollider>().size;
}