using UnityEngine;

public struct ProductionBuildingPrototype {
    public int uniqueId;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject visualsPrefab;
    public PhysicsObstacle dimensionsPrefab;
    public RagdollObstacle physicsObstaclePrefab;
    public ProductionBuildingConfig config;
    public SpawnSpot spawnSpot;
    public SpawnVariant spawnVariant;
    public CombatAgentPrototype combatAgentPrototype;
}