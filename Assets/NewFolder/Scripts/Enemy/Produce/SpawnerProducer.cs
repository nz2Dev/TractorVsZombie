public class SpawnerProducer : IProducer {

    private SpawnerId spawnerId;
    private readonly SpawnerPrototype spawnerPrototype;
    private readonly SpawnerController spawnerController;
    private readonly SpawnSetup initSetup;

    public SpawnerProducer(SpawnerPrototype spawnerPrototype, SpawnerController spawnerController, SpawnSetup initSetup) {
        this.spawnerPrototype = spawnerPrototype;
        this.spawnerController = spawnerController;
        this.initSetup = initSetup;
    }

    public bool IsValid() {
        return true; // TODO: workaround
    }

    public void SpawnEntity() {
        spawnerId = spawnerController.Create(spawnerPrototype);
        spawnerController.Configure(spawnerId, initSetup);
    }

    public bool TryGetSpawnResult(out SpawnResult spawnResult) {
        spawnResult = spawnerController.GetLastSpawnResult(spawnerId);
        return spawnResult != null;
    }

}