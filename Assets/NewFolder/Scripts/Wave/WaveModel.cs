public class WaveModel {

    public WaveModel(int id, WaveConfig config, SpawnPoint spawnPoint) {
        Id = id;
        Config = config;
        SpawnPoint = spawnPoint;
    }

    public int Id { get; }
    public WaveConfig Config { get; }
    public SpawnPoint SpawnPoint { get; }

    public int Queue { get; set; }
    public float NextSpawnTime { get; set; }
    public SpawnResult SpawnResult { get; set; }
    
}