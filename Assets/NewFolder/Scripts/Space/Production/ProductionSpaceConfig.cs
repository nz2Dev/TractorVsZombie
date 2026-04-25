using UnityEngine;

[CreateAssetMenu(fileName = "ProductionSpaceConfig", menuName = "ProductionSpaceConfig", order = 0)]
public class ProductionSpaceConfig : ScriptableObject {
    public int spawnInterval;
    public int initialQueue;
}