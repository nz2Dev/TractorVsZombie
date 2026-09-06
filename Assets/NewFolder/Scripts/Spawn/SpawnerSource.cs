using System;

[Serializable]
public struct SpawnerSource {
    [Inline] public SpawnerConfig config;
    public SpawnSpotSource spawnSpotSource;
    public SpawnVariantSource spawnVariantSource;

    public SpawnerPrototype Get() {
        return new SpawnerPrototype {
            config = config,
            spawnSpot = spawnSpotSource.Get(),
            spawnVariant = spawnVariantSource.Get()
        };
    }
}