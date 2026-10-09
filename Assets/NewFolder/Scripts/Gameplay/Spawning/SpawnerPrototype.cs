public struct SpawnerPrototype {
    public SpawnerConfig config;
    public SpawnPrototype spawnPrototype;

    public SpawnerPrototype(SpawnerConfig config, SpawnPrototype spawnPrototype) {
        this.config = config;
        this.spawnPrototype = spawnPrototype;
    }
}