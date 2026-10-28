using System.Collections.Generic;

public class SpawnerModel {

    public SpawnerModel(SpawnerId id) {
        Id = id;
    }

    public SpawnerId Id { get; }

    public SpawnerConfig Config { get; set; }
    public SpawnPrototype SpawnPrototype { get; set; }

    public int SpawnCount { get; set; }
    public float SpawnInterval { get; set; }
    public float NextSpawnTime { get; set; }
    public SpawnResult LastSpawnEvent { get; set; }
    
}