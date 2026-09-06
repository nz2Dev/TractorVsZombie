using System.Collections.Generic;

public class SpawnerModel {

    public SpawnerModel(SpawnerId id, SpawnerConfig config, SpawnSpot spawnSpot, SpawnVariant spawnVariant) {
        Id = id;
        Config = config;
        SpawnSpot = spawnSpot;
        SpawnVariant = spawnVariant;
    }

    public SpawnerId Id { get; }
    public SpawnerConfig Config { get; }
    public SpawnSpot SpawnSpot { get; }
    public SpawnVariant SpawnVariant { get; }

    public int Queue { get; set; }
    public float SpawnInterval { get; set; }
    public float NextSpawnTime { get; set; }
    public SpawnResult LastSpawnEvent { get; set; }
    public List<int> IdsBuffer { get; } = new(32);
    
}