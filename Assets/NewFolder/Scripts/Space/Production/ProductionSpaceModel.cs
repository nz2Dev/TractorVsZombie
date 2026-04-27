public class ProductionSpaceModel {

    public ProductionSpaceModel(int id, ProductionSpaceConfig config, SpawnSpot spawnSpot, SpawnVariant spawnVariant) {
        Id = id;
        Config = config;
        SpawnSpot = spawnSpot;
        SpawnVariant = spawnVariant;
    }

    public int Id { get; }
    public ProductionSpaceConfig Config { get; }
    public SpawnSpot SpawnSpot { get; }
    public SpawnVariant SpawnVariant { get; }

    public int Queue { get; set; }
    public float NextSpawnTime { get; set; }
    public SpawnResult LastSpawnEvent { get; set; }

}