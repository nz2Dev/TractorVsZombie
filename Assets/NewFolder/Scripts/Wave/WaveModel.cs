public class WaveModel {
    
    public WaveModel(int id, WaveConfig config) {
        Id = id;
        Config = config;
    }

    public int Id { get; }
    public WaveConfig Config { get; }
    public int Queue { get; set; }
    public float NextSpawnTime { get; set; }
    public SpawnResult SpawnResult { get; set; }
    
}