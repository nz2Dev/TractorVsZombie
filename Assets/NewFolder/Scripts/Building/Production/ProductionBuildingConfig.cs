using System;

using UnityEngine;

[CreateAssetMenu(fileName = "Production Building Config", menuName = "ProductionBuildingConfig", order = 0)]
public class ProductionBuildingConfig : ScriptableObject {
    public float spawnInterval = 5f;
    public int initialQueueAmount = 1000;
}
