using System.Collections.Generic;

public class SpawnerModel {

    public SpawnerModel(SpawnerId id) {
        Id = id;
    }

    public SpawnerId Id { get; }

    public SpawnSpot SpawnSpot { get; set; }
    public SpawnConfig SpawnConfig { get; set; }
    public SpawnVariant SpawnVariant { get; set; }

    public int SpawnCount { get; set; }
    public float SpawnInterval { get; set; }
    public float NextSpawnTime { get; set; }
    public SpawnResult LastSpawnEvent { get; set; }
    
    public List<int> IdsBuffer { get; } = new(32);
    
}