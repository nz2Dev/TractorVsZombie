using System;

using UnityEngine;

[CreateAssetMenu(fileName = "Production Building Config", menuName = "ProductionBuildingConfig", order = 0)]
public class ProductionBuildingConfig : ScriptableObject {
    public float spawnInterval = 5f;
    public int initialQueueAmount = 1000;
    [Space]
    public bool alie = false;
    public int maxHealth = 100;
    public float height = 2f;
    public float radius = 1f;
}
