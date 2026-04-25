public class ProductionSpaceModel {

    public ProductionSpaceModel(int id, ProductionSpaceConfig config, SpawnSpot spawnSpot) {
        Id = id;
        Config = config;
        SpawnSpot = spawnSpot;
    }

    public int Id { get; }
    public ProductionSpaceConfig Config { get; }
    public SpawnSpot SpawnSpot { get; }

    public int Queue { get; set; }
    public float NextSpawnTime { get; set; }
    public SpawnResult LastSpawnEvent { get; set; }

}