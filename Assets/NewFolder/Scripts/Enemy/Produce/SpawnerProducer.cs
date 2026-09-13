public class SpawnerProducer : IProducer {

    private SpawnerId spawnerId;
    private readonly SpawnerPrototype spawnerPrototype;
    private readonly SpawnerController spawnerController;

    public SpawnerProducer(SpawnerPrototype spawnerPrototype, SpawnerController spawnerController) {
        this.spawnerPrototype = spawnerPrototype;
        this.spawnerController = spawnerController;
    }

    public bool IsValid() {
        return true; // TODO: workaround
    }

    public void SpawnEntity() {
        spawnerId = spawnerController.Create(spawnerPrototype);
    }

    public bool TryGetSpawnResult(out SpawnResult spawnResult) {
        spawnResult = spawnerController.GetLastSpawnResult(spawnerId);
        return spawnResult != null;
    }

}