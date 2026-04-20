using UnityEngine;

public struct SpawnRequest {
    public int amount;
    public Vector3 position;
    public Quaternion rotation;
    public bool alie;
    public SpawnShape shape;
    public SpawnType spawnType;
    public SpawnConfig spawnConfig;
}