using System.Collections.Generic;

using UnityEngine;

public class SpawnModel {
    
    public SpawnModel(int id, SpawnConfig spawnConfig, Vector3 position) {
        Id = id;
        SpawnConfig = spawnConfig;
        Position = position;
        SpawnedIds = new();
    }

    public int Id {get; private set; }
    public SpawnConfig SpawnConfig { get; private set; }
    public Vector3 Position { get; set; }
    public float LastSpawnTime { get; set; }
    public List<int> SpawnedIds { get; private set; }
}