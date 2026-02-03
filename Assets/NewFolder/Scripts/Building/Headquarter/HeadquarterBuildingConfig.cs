using UnityEngine;

[CreateAssetMenu(fileName = "HeadquarterBuildingConfig", menuName = "HeadquarterBuildingConfig", order = 0)]
public class HeadquarterBuildingConfig : ScriptableObject {
    public int radius = 3;
    public int maxHealth = 100;
    public GameObject visualsPrefab;
}