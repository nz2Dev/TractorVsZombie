public class WaveModel {

    public WaveModel(int id, WaveConfig config, SpawnPoint spawnPoint, SpawnShape spawnShapePrefab) {
        Id = id;
        Config = config;
        SpawnPointA = spawnPoint;
        SpawnShapeAPrefab = spawnShapePrefab;
    }

    public int Id { get; }

    // the config itself is another mechanism for data authroing via scriptable object
    public WaveConfig Config { get; }

    // **
    // how this data is obtained is yet another way of data authroing, provided by scene serialization, they polled from SpawnPointSource : MonoBehavior
    // **
    // this and WaveConfig.spawnConfig/spawnType looks like should be together
    // is all a part of one structure, but not a whole SpawnRequest.
    // Its Authoring is just a data provider
    public SpawnPoint SpawnPointA { get; }
    public SpawnShape SpawnShapeAPrefab { get; }

    // the third example is missing, is how to reference another entity, which will be another way of autoring an external ID
    // because prealocation of ids is a bit complex, but maybe at least should be investigated timeboxed
    // -- public int referencedEntityId / public string entityIdReference

    public int Queue { get; set; }
    public float NextSpawnTime { get; set; }
    public SpawnResult SpawnResult { get; set; }
    
}